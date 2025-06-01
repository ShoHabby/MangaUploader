using System.Globalization;
using System.Text;
using Config.Net;

namespace MangaUploader.Core.Settings.Parsers;

/// <summary>
/// Generic list value parser
/// </summary>
/// <param name="separator">Serialization separator</param>
/// <typeparam name="T">List element</typeparam>
public class ListParser<T>(string separator = " ") : ITypeParser where T : ISpanParsable<T>, IFormattable
{
    #region Properties
    /// <inheritdoc />
    public IEnumerable<Type> SupportedTypes => [typeof(List<T>)];
    /// <summary>
    /// Format stringbuilder
    /// </summary>
    private StringBuilder Builder { get; } = new(128);
    #endregion

    #region Methods
    /// <inheritdoc />
    public bool TryParse(string? value, Type t, out object? result)
    {
        // If null or empty, return as an empty list
        if (string.IsNullOrEmpty(value))
        {
            result = new List<T>();
            return true;
        }

        // Check how many times the delimiter happens and warmup the lists with it
        ReadOnlySpan<char> span = value;
        int count = span.Count(separator) + 1;
        Span<Range> splitRanges = stackalloc Range[count];
        count = span.Split(splitRanges, separator);
        List<T> parsed = new(count);
        for (int i = 0; i < count; i++)
        {
            // If it fails to parse, error out
            if (!T.TryParse(span[splitRanges[i]], CultureInfo.InvariantCulture, out T? parseResult))
            {
                result = null;
                return false;
            }

            parsed.Add(parseResult);
        }

        // Return fully parsed list
        result = parsed;
        return true;
    }

    /// <inheritdoc />
    public string? ToRawString(object? value)
    {
        // Default cases
        if (value is null) return string.Empty;
        if (value is not List<T> list) return null;
        if (list.Count is 0) return string.Empty;

        // Build the string
        this.Builder.Append(list[0].ToString(null, CultureInfo.InvariantCulture));
        for (int i = 1; i < list.Count; i++)
        {
            this.Builder.Append(separator).Append(list[i].ToString(null, CultureInfo.InvariantCulture));
        }

        // Output result
        string result = this.Builder.ToString();
        this.Builder.Clear();
        return result;
    }
    #endregion
}