using Config.Net;

namespace MangaUploader.Core.Settings;

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
    /// Saved GitHub UserID
    /// </summary>
    [Option(Alias = "GitHub.UserID", DefaultValue = null)]
    long? SavedUserID { get; set; }
    /// <summary>
    /// Saved GitHub repository name
    /// </summary>
    [Option(Alias = "GitHub.RepoID", DefaultValue = null)]
    long? SavedRepositoryID { get; set; }
    /// <summary>
    /// Saved GitHub known users
    /// </summary>
    [Option(Alias = "GitHub.KnownUsers", DefaultValue = "")]
    List<long> KnownUserIDs { get; }
    #endregion
}
