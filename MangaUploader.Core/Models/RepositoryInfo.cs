namespace MangaUploader.Core.Models;

/// <summary>
/// User repository info
/// </summary>
/// <param name="Name">Repository name</param>
/// <param name="ID">Repository ID</param>
public readonly record struct RepositoryInfo(string Name, long ID)
{
    #region Methods
    /// <summary>
    /// Repository display string
    /// </summary>
    /// <returns>The display string for this Repository</returns>
    public override string ToString() => this.Name;
    #endregion
}
