using MangaUploader.Core.Models.Cubari;

namespace MangaUploader.Core.Services;

/// <summary>
/// Cubari operations interface
/// </summary>
public interface ICubariService
{
    #region Methods
    /// <summary>
    /// Deserializes a manga object from a string
    /// </summary>
    /// <param name="data">Data to deserialize</param>
    /// <returns>The deserialized manga, or <see langword="null"/> if deserialization was unsuccessful</returns>
    Task<Manga?> DeserializeManga(string data);

    /// <summary>
    /// Serializes a manga object to a string
    /// </summary>
    /// <param name="manga">Object to serialize</param>
    /// <returns>The serialized manga as a string, or null if serialization was unsuccessful</returns>
    Task<string?> SerializeManga(Manga manga);
    #endregion
}
