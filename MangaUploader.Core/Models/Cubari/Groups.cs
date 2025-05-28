using System.Diagnostics;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MangaUploader.Core.Json.Converters;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Group entry model
/// </summary>
[PublicAPI, JsonConverter(typeof(GroupsConverter)), DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Groups
{
    #region Properties
    /// <summary>
    /// DebuggerDisplay
    /// </summary>
    [JsonIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => string.Join(", ", this.Names);

    /// <summary>
    /// Collection of groups who participated in the chapter entry
    /// </summary>
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public List<string> Names { get; } = [];
    #endregion
}
