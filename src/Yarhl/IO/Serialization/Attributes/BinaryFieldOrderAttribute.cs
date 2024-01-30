namespace Yarhl.IO.Serialization.Attributes;

using System;

/// <summary>
/// Specify the order to serialize or deserialize the fields in binary format.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class BinaryFieldOrderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryFieldOrderAttribute"/> class.
    /// </summary>
    /// <param name="order">The order of the field in the binary serialization.</param>
    /// <exception cref="ArgumentOutOfRangeException">The order is less than 0.</exception>
    public BinaryFieldOrderAttribute(int order)
    {
        if (order < 0) {
            throw new ArgumentOutOfRangeException(nameof(order));
        }

        Order = order;
    }

    /// <summary>
    /// Gets or sets the order of the field in the binary format.
    /// </summary>
    public int Order { get; set; }
}
