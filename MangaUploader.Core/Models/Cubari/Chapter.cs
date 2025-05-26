using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MangaUploader.Core.Converters;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Manga chapter model
/// </summary>
[PublicAPI]
public sealed class Chapter
{
    #region Properties
    /// <summary>
    /// Chapter title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Volume number
    /// </summary>
    [JsonConverter(typeof(NullableNumberToStringConverter<decimal>))]
    public decimal? Volume { get; set; }
    /// <summary>
    /// Chapter last updated timestamp
    /// </summary>
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UnixEpoch;
    /// <summary>
    /// Chapter entries per group
    /// </summary>
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public OrderedDictionary<string, Entry> Groups { get; } = [];
    #endregion
}
