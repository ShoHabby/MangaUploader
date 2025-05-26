using System.Text.RegularExpressions;
using FastEnumUtility;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Proxy chapter entry model
/// </summary>
public sealed partial class ProxyEntry : Entry
{
    /// <summary>
    /// Proxy types
    /// </summary>
    public enum ProxyType
    {
        None,
        ImgChest
    }

    #region Constants
    /// <summary>
    /// Proxy entry regex match
    /// </summary>
    [GeneratedRegex(@"/proxy/api/([a-z]+)/chapter/([\w-]+)", RegexOptions.Compiled)]
    private static partial Regex ProxyRegex { get; }
    #endregion

    #region Properties
    /// <summary>
    /// Entry proxy type
    /// </summary>
    public ProxyType Type { get; set; } = ProxyType.None;
    /// <summary>
    /// Entry proxy ID
    /// </summary>
    public string ID { get; set; } = string.Empty;
    #endregion

    #region Methods
    /// <summary>
    /// Converts this ProxyEntry to a a Uri string
    /// </summary>
    /// <returns>The Uri string representation of this proxy entry</returns>
    public string ToUri() => this.Type is not ProxyType.None ? $"/proxy/api/{this.Type.FastToString().ToLowerInvariant()}/chapter/{this.ID}" : string.Empty;
    #endregion

    #region Static Methods
    /// <summary>
    /// Creates a ProxyEntry from a given uri string
    /// </summary>
    /// <param name="uri">Uri to create the entry from</param>
    /// <returns>The created entry, or <see langword="null"/> if an entry could not be created</returns>
    public static ProxyEntry? FromUri(string? uri)
    {
        if (string.IsNullOrEmpty(uri)) return null;

        Match match = ProxyRegex.Match(uri);
        if (match.Success && FastEnum.TryParse(match.Captures[1].ValueSpan, true, out ProxyType type))
        {
            return new ProxyEntry
            {
                Type = type,
                ID   = match.Captures[2].Value
            };
        }

        return null;

    }
    #endregion
}
