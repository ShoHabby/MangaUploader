namespace MangaUploader.Core.Models;

public record RepositoryInfo(string Name, long ID)
{
    public override string ToString() => this.Name;
}
