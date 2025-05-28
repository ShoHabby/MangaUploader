using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MangaUploader.Core.Json.Converters;

/// <summary>
/// Unix timestamp converter
/// </summary>
public sealed class UnixTimestampConverter : JsonConverter<DateTimeOffset>
{
    #region Overrides
    /// <inheritdoc />
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (value is null) throw new JsonException("Could not read string.");

        return long.TryParse(value, out long timestamp) ? DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime() : DateTimeOffset.UnixEpoch.ToLocalTime();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));
    }
    #endregion
}
