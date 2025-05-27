namespace MangaUploader.Core.Models;

/// <summary>
/// User repository info
/// </summary>
/// <param name="Name">Repository name</param>
/// <param name="ID">Repository ID</param>
public readonly record struct RepositoryInfo(string Name, long ID)
{
    #region Overrides
    /// <summary>
    /// String representation of the Repository
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.Name;
    #endregion
}
