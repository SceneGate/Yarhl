namespace Yarhl.IO.Serialization;

using System;
using System.IO;
using System.Text;
using Yarhl.IO.Serialization.Attributes;

/// <summary>
/// Binary deserialization of objects based on their attributes. Equivalent of
/// converting a binary format into an object.
/// </summary>
public class BinaryDeserializer
{
    private readonly DataReader reader;
    private readonly ITypeFieldNavigator fieldNavigator;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryDeserializer"/> class.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    public BinaryDeserializer(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        reader = new DataReader(stream);
        fieldNavigator = new DefaultTypePropertyNavigator();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryDeserializer"/> class.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="fieldNavigator">The strategy to iterate the field's type.</param>
    public BinaryDeserializer(Stream stream, ITypeFieldNavigator fieldNavigator)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(fieldNavigator);

        reader = new DataReader(stream);
        this.fieldNavigator = fieldNavigator;
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
        object obj = Activator.CreateInstance(objType)
            ?? throw new FormatException("Nullable types are not supported");

        foreach (FieldInfo fieldInfo in fieldNavigator.IterateFields(objType)) {
            object propertyValue = DeserializePropertyValue(fieldInfo);
            fieldInfo.SetValueFunc(obj, propertyValue);
        }

        return obj;
    }

    private object DeserializePropertyValue(FieldInfo fieldInfo)
    {
        reader.Endianness = DefaultEndianness;
        var endiannessAttr = fieldInfo.GetAttribute<BinaryEndiannessAttribute>();
        if (endiannessAttr is not null) {
            reader.Endianness = endiannessAttr.Mode;
        }

        if (fieldInfo.Type.IsPrimitive) {
            return DeserializePrimitiveField(fieldInfo);
        } else if (fieldInfo.Type.IsEnum) {
            return DeserializeEnumField(fieldInfo);
        } else if (fieldInfo.Type == typeof(string)) {
            return DeserializeStringField(fieldInfo);
        } else {
            return Deserialize(fieldInfo.Type);
        }
    }

    private object DeserializePrimitiveField(FieldInfo fieldInfo)
    {
        if (fieldInfo.Type == typeof(bool)) {
            if (fieldInfo.GetAttribute<BinaryBooleanAttribute>() is not { } boolAttr) {
                throw new FormatException("Properties of type 'bool' must have the attribute BinaryBoolean");
            }

            object value = reader.ReadByType(boolAttr.UnderlyingType);
            return value.Equals(boolAttr.TrueValue);
        }

        if (fieldInfo.Type == typeof(int) && fieldInfo.GetAttribute<BinaryInt24Attribute>() is not null) {
            return reader.ReadInt24();
        }

        return reader.ReadByType(fieldInfo.Type);
    }

    private object DeserializeEnumField(FieldInfo fieldInfo)
    {
        var enumAttr = fieldInfo.GetAttribute<BinaryEnumAttribute>();
        Type underlyingType = enumAttr?.UnderlyingType
            ?? Enum.GetUnderlyingType(fieldInfo.Type);

        object value = reader.ReadByType(underlyingType);
        return Enum.ToObject(fieldInfo.Type, value);
    }

    private string DeserializeStringField(FieldInfo fieldInfo)
    {
        if (fieldInfo.GetAttribute<BinaryStringAttribute>() is not { } stringAttr) {
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
