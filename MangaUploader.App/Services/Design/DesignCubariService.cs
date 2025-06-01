#if DEBUG
using MangaUploader.Core.Models.Cubari;
using MangaUploader.Core.Services;

namespace MangaUploader.Services.Design;

/// <summary>
/// Design time Cubari service
/// </summary>
internal sealed class DesignCubariService : ICubariService
{
    #region Methods
    /// <inheritdoc />
    public Task<Manga?> DeserializeManga(string data) => Task.FromResult<Manga?>(null);

    /// <inheritdoc />
    public Task<string?> SerializeManga(Manga manga) => Task.FromResult<string?>(null);
    #endregion
}
#endif
