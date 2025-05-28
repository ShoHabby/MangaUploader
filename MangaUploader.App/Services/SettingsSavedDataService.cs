using MangaUploader.Core.Services;

namespace MangaUploader.Services;

/// <summary>
/// Settings-backed saved data service
/// </summary>
public class SettingsSavedDataService : ISavedDataService
{
    #region Properties
    /// <inheritdoc />
    public string SavedRepositoryName => Settings.Default.SelectedRepository;
    #endregion
}
