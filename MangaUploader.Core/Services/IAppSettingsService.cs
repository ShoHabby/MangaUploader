namespace MangaUploader.Core.Services;

/// <summary>
/// Application Settings service
/// </summary>
public interface IAppSettingsService
{
    #region Properties
    /// <summary>
    /// Application settings
    /// </summary>
    IAppSettings Settings { get; }
    #endregion
}
