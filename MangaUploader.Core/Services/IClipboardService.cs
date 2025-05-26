using System.Threading.Tasks;

namespace MangaUploader.Core.Services;

/// <summary>
/// Device clipboard service
/// </summary>
public interface IClipboardService
{
    #region Methods
    /// <summary>
    /// Copies a text value to the clipboard
    /// </summary>
    /// <param name="text">Value to copy to the clipboard</param>
    Task CopyTextToClipboard(string text);

    /// <summary>
    /// Gets a text value from the clipboard
    /// </summary>
    /// <returns>The fetched value from the clipboard </returns>
    Task<string?> CopyTextFromClipboard();
    #endregion
}
