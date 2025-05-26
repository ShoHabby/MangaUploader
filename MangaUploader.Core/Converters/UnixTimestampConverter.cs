using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MangaUploader.Core.Converters;

public class UnixTimestampConverter : JsonConverter<DateTimeOffset>
{
    #region Overrides of JsonConverter<DateTime>
    /// <inheritdoc />
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        reader.Read();
        return value is not null && long.TryParse(value, out long timestamp)
                   ? DateTimeOffset.FromUnixTimeSeconds(timestamp) : DateTimeOffset.UnixEpoch;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));
    }
    #endregion
}
