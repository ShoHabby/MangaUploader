using System.Text.Json;
using System.Text.Json.Serialization;
using MangaUploader.Core.Extensions.Json;

namespace MangaUploader.Core.Json.Converters;

/// <summary>
/// Null value to empty string converter
/// </summary>
public sealed class NullToStringConverter<T> : JsonConverter<T?> where T : class
{
    #region Properties
    /// <inheritdoc />
    public override bool HandleNull => true;
    #endregion

    #region Overrides
    /// <inheritdoc />
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.String) throw new JsonException("Expecting string value.");

        string? value = reader.GetString();
        return value switch
        {
            null => throw new JsonException("Could not read string value."),
            ""   => null,
            _    => options.GetConverter<T>().Read(ref reader, typeToConvert, options)
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is not null)
        {
            options.GetConverter<T>().Write(writer, value, options);
        }
        else
        {
            writer.WriteStringValue(string.Empty);
        }
    }
    #endregion
}
