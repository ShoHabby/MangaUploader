using System;
using System.Collections.Generic;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Manga chapter model
/// </summary>
public sealed class Chapter
{
    /// <summary>
    /// Chapter title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Volume number
    /// </summary>
    public decimal? Volume { get; set; }
    /// <summary>
    /// Chapter entries per group
    /// </summary>
    public OrderedDictionary<string, Entry> Groups { get; } = [];
    /// <summary>
    /// Chapter last updated timestamp
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
