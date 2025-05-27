using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace MangaUploader.Core.Extensions.Json;

/// <summary>
/// Json serializing extensions
/// </summary>
public static class JsonExtensions
{
    #region Extension Methods
    /// <summary>
    /// Gets the converter for a specified type
    /// </summary>
    /// <typeparam name="T">Type to get the converter for</typeparam>
    /// <returns>The fetched Json converter for the given type</returns>
    public static JsonConverter<T> GetConverter<T>(this JsonSerializerOptions options) => (JsonConverter<T>)options.GetConverter(typeof(T));
    #endregion
}
