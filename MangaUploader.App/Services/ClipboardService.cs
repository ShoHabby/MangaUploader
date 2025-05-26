using System.Threading.Tasks;
using MangaUploader.Core.Services;

namespace MangaUploader.Services;

/// <summary>
/// Desktop clipboard service
/// </summary>
public sealed class ClipboardService : IClipboardService
{
    #region Methods
    /// <inheritdoc />
    public async Task CopyTextToClipboard(string text)
    {
        // Don't copy null or empty values
        if (!string.IsNullOrEmpty(text))
        {
            await App.GetTopLevel().Clipboard!.SetTextAsync(text);
        }
    }

    /// <inheritdoc />
    public async Task<string?> CopyTextFromClipboard() => await App.GetTopLevel().Clipboard!.GetTextAsync();
    #endregion
}
