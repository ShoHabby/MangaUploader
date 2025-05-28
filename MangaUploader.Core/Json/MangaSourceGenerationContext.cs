using System.Text.Json.Serialization;
using MangaUploader.Core.Models.Cubari;

namespace MangaUploader.Core.Json;

/// <summary>
/// Cubari Manga model source generation json deserialization context
/// </summary>
[JsonSourceGenerationOptions(NumberHandling       = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                             PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
                             WriteIndented        = true)]
[JsonSerializable(typeof(Manga))]
public sealed partial class MangaSourceGenerationContext : JsonSerializerContext;
