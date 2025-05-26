using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MangaUploader.Core.Converters;

/// <summary>
/// Converter handling nullable numbers
/// </summary>
/// <typeparam name="T">Number type</typeparam>
public sealed class NullableNumberToStringConverter<T> : JsonConverter<T?> where T : struct, INumber<T>
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
            _    => T.TryParse(value, CultureInfo.InvariantCulture, out T result) ? result : throw new JsonException("Could not parse number correctly.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
    #endregion
}
