namespace Yarhl.IO.Serialization;

using System;
using System.IO;
using System.Linq;
using System.Text;
using Yarhl.IO.Serialization.Attributes;

/// <summary>
/// Binary serialization of objects based on attributes. Equivalent to convert
/// an object into binary.
/// </summary>
public class BinarySerializer
{
    private readonly ITypeFieldNavigator fieldNavigator;
    private readonly DataWriter writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinarySerializer"/> class.
    /// </summary>
    /// <param name="stream">The stream to write the binary data.</param>
    public BinarySerializer(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        writer = new DataWriter(stream);
        fieldNavigator = new DefaultTypePropertyNavigator();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinarySerializer"/> class.
    /// </summary>
    /// <param name="stream">The stream to write the binary data.</param>
    /// <param name="fieldNavigator">Strategy to iterate over type fields.</param>
    public BinarySerializer(Stream stream, ITypeFieldNavigator fieldNavigator)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(fieldNavigator);

        writer = new DataWriter(stream);
        this.fieldNavigator = fieldNavigator;
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
        foreach (FieldInfo property in fieldNavigator.IterateFields(type)) {
            SerializeProperty(property, obj);
        }
    }

    private void SerializeProperty(FieldInfo fieldInfo, object obj)
    {
        writer.Endianness = DefaultEndianness;
        var endiannessAttr = fieldInfo.GetAttribute<BinaryEndiannessAttribute>();
        if (endiannessAttr is not null) {
            writer.Endianness = endiannessAttr.Mode;
        }

        object value = fieldInfo.GetValueFunc(obj)
            ?? throw new FormatException("Cannot serialize nullable values");

        if (fieldInfo.Type.IsPrimitive) {
            SerializePrimitiveField(fieldInfo, value);
        } else if (fieldInfo.Type.IsEnum) {
            SerializeEnumField(fieldInfo, value);
        } else if (fieldInfo.Type == typeof(string)) {
            SerializeString(fieldInfo, value);
        } else {
            Serialize(fieldInfo.Type, value);
        }
    }

    private void SerializePrimitiveField(FieldInfo fieldInfo, object value)
    {
        // Handle first the special cases
        if (fieldInfo.Type == typeof(bool)) {
            if (fieldInfo.GetAttribute<BinaryBooleanAttribute>() is not { } boolAttr) {
                throw new FormatException("Properties of type 'bool' must have the attribute BinaryBoolean");
            }

            object typeValue = (bool)value ? boolAttr.TrueValue : boolAttr.FalseValue;
            writer.WriteOfType(boolAttr.UnderlyingType, typeValue);
            return;
        }

        if (fieldInfo.Type == typeof(int) && fieldInfo.Attributes.Any(a => a is BinaryInt24Attribute)) {
            writer.WriteInt24((int)value);
            return;
        }

        // Fallback to DataWriter primitive write
        writer.WriteOfType(fieldInfo.Type, value);
    }

    private void SerializeEnumField(FieldInfo fieldInfo, object value)
    {
        var enumAttr = fieldInfo.GetAttribute<BinaryEnumAttribute>();
        Type underlyingType = enumAttr?.UnderlyingType
            ?? Enum.GetUnderlyingType(fieldInfo.Type);

        writer.WriteOfType(underlyingType, value);
    }

    private void SerializeString(FieldInfo fieldInfo, object value)
    {
        if (fieldInfo.GetAttribute<BinaryStringAttribute>() is not { } stringAttr) {
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
