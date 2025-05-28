using MangaUploader.Core;
using MangaUploader.Core.Services;

namespace MangaUploader.Services;

/// <summary>
/// Configuration-built Application Settings service
/// </summary>
public sealed class AppSettingsService : IAppSettingsService
{
    #region Properties
    /// <inheritdoc />
    public IAppSettings Settings => App.Settings;
    #endregion
}
