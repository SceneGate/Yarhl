namespace Yarhl.IO.Serialization;

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Yarhl.IO.Serialization.Attributes;

/// <summary>
/// Binary deserialization of objects based on their attributes. Equivalent of
/// converting a binary format into an object.
/// </summary>
public class BinaryDeserializer
{
    private readonly DataReader reader;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryDeserializer"/> class.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    public BinaryDeserializer(Stream stream)
    {
        reader = new DataReader(stream);
    }

    /// <summary>
    /// Gets or sets the default endianness for the deserialization.
    /// </summary>
    public EndiannessMode DefaultEndianness { get; set; }

    /// <summary>
    /// Deserialize an object from the binary data of the stream.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>A new object deserialized.</returns>
    public static T Deserialize<T>(Stream stream)
    {
        return new BinaryDeserializer(stream).Deserialize<T>();
    }

    /// <summary>
    /// Deserialize an object from the binary data of the stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="objType">The type of the object to deserialize.</param>
    /// <returns>A new object deserialized.</returns>
    public static object Deserialize(Stream stream, Type objType)
    {
        return new BinaryDeserializer(stream).Deserialize(objType);
    }

    /// <summary>
    /// Deserialize an object from the binary data of the stream.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <returns>A new object deserialized.</returns>
    public T Deserialize<T>()
    {
        return (T)Deserialize(typeof(T));
    }

    /// <summary>
    /// Deserialize an object from the binary data of the stream.
    /// </summary>
    /// <param name="objType">The type of the object to deserialize.</param>
    /// <returns>A new object deserialized.</returns>
    public object Deserialize(Type objType)
    {
        // It returns null for Nullable<T>, but as that is a class and
        // it won't have the serializable attribute, it will throw an
        // unsupported exception before. So this can't be null at this point.
        object obj = Activator.CreateInstance(objType)!;

        PropertyInfo[] properties = objType.GetProperties(
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |
            BindingFlags.Instance);

        foreach (PropertyInfo property in properties) {
            bool ignore = Attribute.IsDefined(property, typeof(BinaryIgnoreAttribute));
            if (ignore) {
                continue;
            }

            object propertyValue = DeserializePropertyValue(property);
            property.SetValue(obj, propertyValue);
        }

        return obj;
    }

    private object DeserializePropertyValue(PropertyInfo property)
    {
        reader.Endianness = DefaultEndianness;
        var endiannessAttr = property.GetCustomAttribute<BinaryForceEndiannessAttribute>();
        if (endiannessAttr is not null) {
            reader.Endianness = endiannessAttr.Mode;
        }

        if (property.PropertyType.IsPrimitive) {
            return DeserializePrimitiveField(property);
        } else if (property.PropertyType.IsEnum) {
            return DeserializeEnumField(property);
        } else if (property.PropertyType == typeof(string)) {
            return DeserializeStringField(property);
        } else {
            return Deserialize(property.PropertyType);
        }
    }

    private object DeserializePrimitiveField(PropertyInfo property)
    {
        if (property.PropertyType == typeof(bool)) {
            if (property.GetCustomAttribute<BinaryBooleanAttribute>() is not { } boolAttr) {
                throw new FormatException("Properties of type 'bool' must have the attribute BinaryBoolean");
            }

            object value = reader.ReadByType(boolAttr.UnderlyingType);
            return value.Equals(boolAttr.TrueValue);
        }

        if (property.PropertyType == typeof(int) && Attribute.IsDefined(property, typeof(BinaryInt24Attribute))) {
            return reader.ReadInt24();
        }

        return reader.ReadByType(property.PropertyType);
    }

    private object DeserializeEnumField(PropertyInfo property)
    {
        var enumAttr = property.GetCustomAttribute<BinaryEnumAttribute>();
        Type underlyingType = enumAttr?.UnderlyingType
            ?? Enum.GetUnderlyingType(property.PropertyType);

        object value = reader.ReadByType(underlyingType);
        return Enum.ToObject(property.PropertyType, value);
    }

    private string DeserializeStringField(PropertyInfo property)
    {
        if (property.GetCustomAttribute<BinaryStringAttribute>() is not { } stringAttr) {
            // Use default settings if not specified.
            return reader.ReadString();
        }

        Encoding? encoding = null;
        if (stringAttr!.CodePage != -1) {
            encoding = Encoding.GetEncoding(stringAttr.CodePage);
        }

        if (stringAttr.SizeType is null) {
            return (stringAttr.FixedSize == -1)
                ? reader.ReadString(encoding)
                : reader.ReadString(stringAttr.FixedSize, encoding);
        }

        return reader.ReadString(stringAttr.SizeType, encoding);
    }
}
