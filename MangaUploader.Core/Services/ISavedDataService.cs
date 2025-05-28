namespace MangaUploader.Core.Services;

/// <summary>
/// Service offering saved user data
/// </summary>
public interface ISavedDataService
{
    /// <summary>
    /// Last saved repository name
    /// </summary>
    string SavedRepositoryName { get; }
}
