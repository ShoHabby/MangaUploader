namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Proxy chapter entry model
/// </summary>
public sealed class ProxyEntry : Entry
{
    /// <summary>
    /// Proxy types
    /// </summary>
    public enum ProxyType
    {
        None,
        ImageChest
    }

    /// <summary>
    /// Entry proxy type
    /// </summary>
    public ProxyType Type { get; set; } = ProxyType.None;
    /// <summary>
    /// Entry proxy ID
    /// </summary>
    public string ID { get; set; } = string.Empty;
}
