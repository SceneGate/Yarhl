namespace Yarhl.IO.Serialization;

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Yarhl.IO.Serialization.Attributes;

/// <summary>
/// Binary serialization of objects based on attributes. Equivalent to convert
/// an object into binary.
/// </summary>
public class BinarySerializer
{
    private readonly DataWriter writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinarySerializer"/> class.
    /// </summary>
    /// <param name="stream">The stream to write the binary data.</param>
    public BinarySerializer(Stream stream)
    {
        writer = new DataWriter(stream);
    }

    /// <summary>
    /// Gets or sets the default endianness for the serialization.
    /// </summary>
    public EndiannessMode DefaultEndianness { get; set; }

    /// <summary>
    /// Serialize the public properties of the object in binary data in the stream.
    /// </summary>
    /// <param name="stream">The stream to write the binary data.</param>
    /// <param name="obj">The object to serialize into the stream.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    public static void Serialize<T>(Stream stream, T obj)
    {
        new BinarySerializer(stream).Serialize(obj);
    }

    /// <summary>
    /// Serialize the public properties of the object in binary data in the stream.
    /// </summary>
    /// <param name="stream">The stream to write the binary data.</param>
    /// <param name="objType">The type of object to serialize.</param>
    /// <param name="obj">The object to serialize into the stream.</param>
    public static void Serialize(Stream stream, Type objType, object obj)
    {
        new BinarySerializer(stream).Serialize(objType, obj);
    }

    /// <summary>
    /// Serialize the public properties of the object in binary data in the stream.
    /// </summary>
    /// <param name="obj">The object to serialize into the stream.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    public void Serialize<T>(T obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        Serialize(typeof(T), obj);
    }

    /// <summary>
    /// Serialize the public properties of the object in binary data in the stream.
    /// </summary>
    /// <param name="type">The type of object to serialize.</param>
    /// <param name="obj">The object to serialize into the stream.</param>
    public void Serialize(Type type, object obj)
    {
        PropertyInfo[] properties = type.GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance);

        // TODO: Introduce property to sort
        foreach (PropertyInfo property in properties) {
            bool ignore = Attribute.IsDefined(property, typeof(BinaryIgnoreAttribute));
            if (ignore) {
                continue;
            }

            SerializeProperty(property, obj);
        }
    }

    private void SerializeProperty(PropertyInfo property, object obj)
    {
        writer.Endianness = DefaultEndianness;
        var endiannessAttr = property.GetCustomAttribute<BinaryEndiannessAttribute>();
        if (endiannessAttr is not null) {
            writer.Endianness = endiannessAttr.Mode;
        }

        object value = property.GetValue(obj)
            ?? throw new FormatException("Cannot serialize nullable values");

        if (property.PropertyType.IsPrimitive) {
            SerializePrimitiveField(property, value);
        } else if (property.PropertyType.IsEnum) {
            var enumAttr = property.GetCustomAttribute<BinaryEnumAttribute>();
            Type underlyingType = enumAttr?.UnderlyingType
                ?? Enum.GetUnderlyingType(property.PropertyType);

            writer.WriteOfType(underlyingType, value);
        } else if (property.PropertyType == typeof(string)) {
            SerializeString(property, value);
        } else {
            Serialize(property.PropertyType, value);
        }
    }

    private void SerializePrimitiveField(PropertyInfo property, object value)
    {
        // Handle first the special cases
        if (property.PropertyType == typeof(bool)) {
            if (property.GetCustomAttribute<BinaryBooleanAttribute>() is not { } boolAttr) {
                throw new FormatException("Properties of type 'bool' must have the attribute BinaryBoolean");
            }

            object typeValue = (bool)value ? boolAttr.TrueValue : boolAttr.FalseValue;
            writer.WriteOfType(boolAttr.UnderlyingType, typeValue);
            return;
        }

        if (property.PropertyType == typeof(int) && Attribute.IsDefined(property, typeof(BinaryInt24Attribute))) {
            writer.WriteInt24((int)value);
            return;
        }

        // Fallback to DataWriter primitive write
        writer.WriteOfType(property.PropertyType, value);
    }

    private void SerializeString(PropertyInfo property, object value)
    {
        if (property.GetCustomAttribute<BinaryStringAttribute>() is not { } stringAttr) {
            // Use default settings if not specified.
            writer.Write((string)value);
            return;
        }

        Encoding? encoding = null;
        if (stringAttr.CodePage != -1) {
            encoding = Encoding.GetEncoding(stringAttr.CodePage);
        }

        string strValue = (string)value;

        if (stringAttr.SizeType is null) {
            if (stringAttr.FixedSize == -1) {
                writer.Write(strValue, stringAttr.Terminator, encoding, stringAttr.MaxSize);
            } else {
                writer.Write(strValue, stringAttr.FixedSize, stringAttr.Terminator, encoding);
            }
        } else {
            writer.Write(strValue, stringAttr.SizeType, stringAttr.Terminator, encoding, stringAttr.MaxSize);
        }
    }
}
