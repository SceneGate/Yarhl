namespace Yarhl.IO.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Information of a field member of a type.
/// </summary>
/// <param name="Name">Name of the field.</param>
/// <param name="Type">Type of the field.</param>
/// <param name="GetValueFunc">Function that returns the fields' value given the object.</param>
/// <param name="SetValueFunc">
/// Function that sets the fields'value on the given object.
/// The first argument is the object and the second the value to set.
/// </param>
/// <param name="Attributes">Optional collection of attributes on the field.</param>
public record FieldInfo(
    string Name,
    Type Type,
    Func<object?, object?> GetValueFunc,
    Action<object?, object?> SetValueFunc,
    IEnumerable<Attribute> Attributes)
{
    /// <summary>
    /// Returns the first attribute if any of the given type.
    /// </summary>
    /// <typeparam name="T">The attribute type to search.</typeparam>
    /// <returns>The attribute on the given type if any or null otherwise.</returns>
    public T? GetAttribute<T>()
        where T : Attribute
    {
        return Attributes.OfType<T>().FirstOrDefault();
    }
}
