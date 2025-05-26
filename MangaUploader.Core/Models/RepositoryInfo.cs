namespace MangaUploader.Core.Models;

public readonly record struct RepositoryInfo(string Name, long ID)
{
    public override string ToString() => this.Name;
}
