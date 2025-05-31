using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MangaUploader.Core.Json;
using MangaUploader.Core.Models.Cubari;
using MangaUploader.Core.Services;
using Microsoft.IO;

namespace MangaUploader.Services;

/// <summary>
/// Cubari API service
/// </summary>
internal sealed class CubariService : ICubariService
{
    #region Constants
    /// <summary>
    /// Memory stream manager
    /// </summary>
    private static RecyclableMemoryStreamManager StreamManager { get; } = new();
    #endregion

    #region Properties
    /// <summary>
    /// Json serializer options
    /// </summary>
    private JsonSerializerOptions Options { get; } = new()
    {
        Encoder              = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling       = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        TypeInfoResolver     = MangaSourceGenerationContext.Default,
        WriteIndented        = true
    };
    #endregion

    #region Methods
    /// <inheritdoc />
    public async Task<Manga?> DeserializeManga(string data)
    {
        try
        {
            await using RecyclableMemoryStream deserializeStream = StreamManager.GetStream(nameof(DeserializeManga), Encoding.UTF8.GetByteCount(data));
            await using StreamWriter writer = new(deserializeStream, Encoding.UTF8);
            await writer.WriteAsync(data);
            await writer.FlushAsync();
            deserializeStream.Seek(0L, SeekOrigin.Begin);
            return await JsonSerializer.DeserializeAsync<Manga>(deserializeStream, this.Options);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<string?> SerializeManga(Manga manga)
    {
        try
        {
            await using RecyclableMemoryStream serializeStream = StreamManager.GetStream(nameof(SerializeManga), 1024L);
            await JsonSerializer.SerializeAsync(serializeStream, manga, this.Options);
            using StreamReader reader = new(serializeStream, Encoding.UTF8);
            serializeStream.Seek(0L, SeekOrigin.Begin);
            return await reader.ReadToEndAsync();
        }
        catch
        {
            return null;
        }
    }
    #endregion
}
