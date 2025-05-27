using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Image list chapter entry model
/// </summary>
[PublicAPI]
public sealed class ListEntry : Entry
{
    #region Properties
    /// <summary>
    /// Entry image URLs
    /// </summary>
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public List<Uri> Images { get; } = [];
    #endregion
}
