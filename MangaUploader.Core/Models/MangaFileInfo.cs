using MangaUploader.Core.Models.Cubari;

namespace MangaUploader.Core.Models;

/// <summary>
/// Manga JSON file info
/// </summary>
/// <param name="Path">File path in repository</param>
/// <param name="SHA">File SHA</param>
/// <param name="RepositoryID">Repository ID</param>
/// <param name="Manga">Manga payload object</param>
public readonly record struct MangaFileInfo(string Path, string SHA, long RepositoryID, Manga Manga);
