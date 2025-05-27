using System.Text.Json.Serialization;
using MangaUploader.Core.Converters;

namespace MangaUploader.Core.Models.Cubari;

/// <summary>
/// Chapter entry model base class
/// </summary>
[JsonConverter(typeof(EntryConverter))]
public abstract class Entry;
