using System;
using System.Collections.Generic;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Image list chapter entry model
/// </summary>
public sealed class ListEntry : Entry
{
    #region Properties
    /// <summary>
    /// Entry image URLs
    /// </summary>
    public List<Uri> Images { get; } = [];
    #endregion
}
