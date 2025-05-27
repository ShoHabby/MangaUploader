using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MangaUploader.Core.Converters;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Manga chapter model
/// </summary>
[PublicAPI, DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Chapter
{
    #region Properties
    /// <summary>
    /// Debugger display
    /// </summary>
    [JsonIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            string display = string.IsNullOrEmpty(this.Title) ? "Chapter" : $"Chapter: \"{this.Title}\"";
            if (this.Volume is not null)
            {
                display = $"Vol. {this.Volume} {display}";
            }
            return display;
        }
    }

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
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UnixEpoch.ToLocalTime();
    /// <summary>
    /// Chapter entries per group
    /// </summary>
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public OrderedDictionary<Groups, Entry> Groups { get; } = [];
    #endregion
}
