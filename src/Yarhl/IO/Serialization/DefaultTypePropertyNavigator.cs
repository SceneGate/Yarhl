namespace Yarhl.IO.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yarhl.IO.Serialization.Attributes;

/// <summary>
/// Field navigator for types that iterate over public non-static properties only.
/// It includes inherited properties. It follows the order given by the order attribute.
/// </summary>
public class DefaultTypePropertyNavigator : ITypeFieldNavigator
{
    /// <inheritdoc />
    public virtual IEnumerable<FieldInfo> IterateFields(Type type)
    {
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && (p.GetGetMethod(false)?.IsPublic ?? false))
            .Where(p => p.CanWrite && (p.GetSetMethod(false)?.IsPublic ?? false))
            .Where(p => p.GetCustomAttribute<BinaryIgnoreAttribute>() is null)
            .ToArray();

        SortProperties(properties);

        foreach (PropertyInfo property in properties) {
            var info = new FieldInfo(
                property.Name,
                property.PropertyType,
                property.GetValue,
                property.SetValue,
                property.GetCustomAttributes());

            yield return info;
        }
    }

    private static void SortProperties(PropertyInfo[] properties)
    {
        int[] orderKeys = properties
            .Select(p => p.GetCustomAttribute<BinaryOrderAttribute>())
            .Where(p => p is not null)
            .Select(p => p!.Order)
            .ToArray();

#if NET6_0
        if (orderKeys.Length != properties.Length) {
            throw new FormatException("Prior .NET 8.0, every property must have the BinaryFieldOrder attribute");
        }

        Array.Sort(orderKeys, properties);
#elif NET8_0_OR_GREATER
        if (orderKeys.Length > 0 && orderKeys.Length != properties.Length) {
            throw new FormatException("BinaryFieldOrder must be applied to none or all properties");
        }

        if (orderKeys.Length > 0) {
            Array.Sort(orderKeys, properties);
        }
#endif
    }
}
