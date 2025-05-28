using Config.Net;

namespace MangaUploader.Core;

/// <summary>
/// Application settings
/// </summary>
public interface IAppSettings
{
    #region Properties
    /// <summary>
    /// Window width
    /// </summary>
    [Option(Alias = "Window.Width", DefaultValue = 600)]
    int WindowWidth { get; set; }
    /// <summary>
    /// Window height
    /// </summary>
    [Option(Alias = "Window.Height", DefaultValue = 800)]
    int WindowHeight { get; set; }
    /// <summary>
    /// Saved GitHub repository name
    /// </summary>
    [Option(Alias = "Repository.Name", DefaultValue = "")]
    string SavedRepositoryName { get; set; }
    #endregion
}
