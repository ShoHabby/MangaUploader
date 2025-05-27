using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MangaUploader.Core.Converters;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Group entry model
/// </summary>
[PublicAPI, JsonConverter(typeof(GroupsConverter))]
public sealed class Groups
{
    #region Properties
    /// <summary>
    /// Collection of groups who participated in the chapter entry
    /// </summary>
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public List<string> Names { get; } = [];
    #endregion
}
