using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MangaUploader.Core.Extensions.Logging;
using MangaUploader.Core.Models.Cubari;
using MangaUploader.Core.Services;

namespace MangaUploader.Services;

/// <summary>
/// Cubari API service
/// </summary>
public class CubariService : ICubariService
{
    #region Properties
    /// <summary>
    /// Json serializer options
    /// </summary>
    private JsonSerializerOptions Options { get; } = new()
    {
        Encoder              = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling       = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented        = true
    };
    #endregion

    #region Methods
    /// <inheritdoc />
    public Manga? DeserializeManga(string data)
    {
        try
        {
            return JsonSerializer.Deserialize<Manga>(data, this.Options);
        }
        catch (Exception e)
        {
            e.LogException();
            return null;
        }
    }

    /// <inheritdoc />
    public string? SerializeManga(Manga manga)
    {
        try
        {
            return JsonSerializer.Serialize(manga, this.Options);
        }
        catch (Exception e)
        {
            e.LogException();
            return null;
        }
    }
    #endregion
}
