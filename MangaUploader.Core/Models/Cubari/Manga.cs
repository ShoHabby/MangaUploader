using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MangaUploader.Core.Converters;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Manga series model
/// </summary>
[PublicAPI, DebuggerDisplay("Manga: \"{Title}\"")]
public sealed partial class Manga
{
    /// <summary>
    /// Cubari Manga model source generation json deserialization context
    /// </summary>
    [JsonSourceGenerationOptions(NumberHandling       = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                                 PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
                                 WriteIndented        = true)]
    [JsonSerializable(typeof(Manga))]
    public partial class MangaSourceGenerationContext : JsonSerializerContext;

    #region Properties
    /// <summary>
    /// Manga title
    /// </summary>
    public required string Title { get; set; }
    /// <summary>
    /// Manga description
    /// </summary>
    public required string Description { get; set; }
    /// <summary>
    /// Artist name
    /// </summary>
    public required string Artist { get; set; }
    /// <summary>
    /// Author name
    /// </summary>
    public required string Author { get; set; }
    /// <summary>
    /// Cover page URL
    /// </summary>
    [JsonConverter(typeof(NullToStringConverter<Uri>))]
    public required Uri? Cover { get; set; }
    /// <summary>
    /// Manga chapters
    /// </summary>
    public required SortedDictionary<decimal, Chapter> Chapters { get; init; }
    #endregion
}
