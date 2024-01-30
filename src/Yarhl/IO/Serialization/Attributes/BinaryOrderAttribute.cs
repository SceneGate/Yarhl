namespace Yarhl.IO.Serialization.Attributes;

using System;

/// <summary>
/// Specify the order to serialize or deserialize the fields in binary format.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class BinaryOrderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryOrderAttribute"/> class.
    /// </summary>
    /// <param name="order">The order of the field in the binary serialization.</param>
    public BinaryOrderAttribute(int order)
    {
        Order = order;
    }

    /// <summary>
    /// Gets or sets the order of the field in the binary format.
    /// </summary>
    public int Order { get; set; }
}
