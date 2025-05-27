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
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Manga description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Artist name
    /// </summary>
    public string Artist { get; set; } = string.Empty;
    /// <summary>
    /// Author name
    /// </summary>
    public string Author { get; set; } = string.Empty;
    /// <summary>
    /// Cover page URL
    /// </summary>
    [JsonConverter(typeof(NullToStringConverter<Uri>))]
    public Uri? Cover { get; set; }
    /// <summary>
    /// Manga chapters
    /// </summary>
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public SortedDictionary<decimal, Chapter> Chapters { get; } = [];
    #endregion
}
