namespace Yarhl.FileFormat;

using System;

/// <summary>
/// Extensions that applies to every format.
/// </summary>
public static class FormatExtensions
{
    /// <summary>
    /// Converts the format using a converter with the specified type.
    /// </summary>
    /// <typeparam name="TSrc">Source format.</typeparam>
    /// <typeparam name="TDst">Destination format.</typeparam>
    /// <param name="src">The input object to convert.</param>
    /// <param name="converter">The converter to use.</param>
    /// <param name="disposeInput">
    /// Value indicating whether it disposes the source input after converting.
    /// </param>
    /// <param name="disposeConverter">
    /// Value indicating whether it disposes the converter after using it.
    /// </param>
    /// <returns>The output from the converter.</returns>
    public static TDst ConvertWith<TSrc, TDst>(
        this TSrc src,
        IConverter<TSrc, TDst> converter,
        bool disposeInput = false,
        bool disposeConverter = false)
            where TSrc : IFormat
    {
        // It looks useless but allows a cleaner fluent-like API:
        // format.ConvertWith(converter1).ConvertWith(converter2)
        TDst result = converter.Convert(src);

        if (disposeInput && src is IDisposable disposeSrc) {
            disposeSrc.Dispose();
        }

        if (disposeConverter && converter is IDisposable disposeConv) {
            disposeConv.Dispose();
        }

        return result;
    }
}
