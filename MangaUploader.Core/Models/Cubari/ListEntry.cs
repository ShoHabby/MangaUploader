using System.Diagnostics;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Image list chapter entry model
/// </summary>
[PublicAPI, DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class ListEntry : Entry
{
    #region Properties
    /// <summary>
    /// Debugger display
    /// </summary>
    [JsonIgnore, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"Count = {this.Images.Count}";

    /// <summary>
    /// Entry image URLs
    /// </summary>
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate), DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public List<Uri> Images { get; } = [];
    #endregion
}
