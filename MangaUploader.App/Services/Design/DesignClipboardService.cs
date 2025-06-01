#if DEBUG
using MangaUploader.Core.Services;

namespace MangaUploader.Services.Design;

/// <summary>
/// Design time clipboard service
/// </summary>
public class DesignClipboardService : IClipboardService
{
    #region Methods
    /// <inheritdoc />
    public Task CopyTextToClipboard(string text) => Task.CompletedTask;

    /// <inheritdoc />
    public Task<string?> CopyTextFromClipboard() => Task.FromResult<string?>(null);
    #endregion
}
#endif
