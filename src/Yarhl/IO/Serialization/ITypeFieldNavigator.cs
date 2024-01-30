namespace Yarhl.IO.Serialization;

using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Interface to provide implementations that iterate over fields of types.
/// </summary>
public interface ITypeFieldNavigator
{
    /// <summary>
    /// Iterate over the fields of a given type using reflection with enumerables.
    /// </summary>
    /// <param name="type">The type to iterate over fields.</param>
    /// <returns>Enumerable to iterate over its fields.</returns>
    IEnumerable<FieldInfo> IterateFields(Type type);
}
