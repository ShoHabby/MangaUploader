using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MangaUploader.Core.Json.Converters;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Manga series model
/// </summary>
[PublicAPI, DebuggerDisplay("Manga: \"{Title}\"")]
public sealed class Manga
{
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
