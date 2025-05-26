using System;
using System.Collections.Generic;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Manga series model
/// </summary>
public sealed class Manga
{
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
    public Uri Cover { get; set; } = new(string.Empty);
    /// <summary>
    /// Manga chapters
    /// </summary>
    public SortedDictionary<decimal, Chapter> Chapters { get; } = [];
}
