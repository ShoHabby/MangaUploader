using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MangaUploader.Core.Models.Cubari;

namespace MangaUploader.Core.Converters;

/// <summary>
/// Groups JSON converter
/// </summary>
public sealed class GroupsConverter : JsonConverter<Groups>
{
    #region Overrides
    /// <inheritdoc />
    public override Groups Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType is JsonTokenType.String ? ReadInternal(reader.GetString()) : throw new JsonException("Expected string.");
    }

    /// <inheritdoc />
    public override Groups ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType is JsonTokenType.PropertyName ? ReadInternal(reader.GetString()) : throw new JsonException("Expected string.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Groups value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(WriteInternal(value));
    }

    /// <inheritdoc />
    public override void WriteAsPropertyName(Utf8JsonWriter writer, Groups value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(WriteInternal(value));
    }
    #endregion

    #region Methods
    /// <summary>
    /// Reads the groups value from a serialized Json version
    /// </summary>
    /// <param name="value">Value to read</param>
    /// <returns>The deserialization of the Groups value</returns>
    private static Groups ReadInternal(string? value)
    {
        // Create empty Groups
        Groups groups = new();
        value = value?.Trim();
        if (string.IsNullOrEmpty(value)) return groups;

        // Check how many groups are in the list
        ReadOnlySpan<char> span = value;
        int count = span.Count(',');
        if (count is 0)
        {
            // Only one group, add the full string
            groups.Names.Add(value);
        }
        else
        {
            // Several groups, split out the name
            foreach (Range splitRange in span.Split(','))
            {
                ReadOnlySpan<char> split = span[splitRange].Trim();
                if (split.Length is not 0)
                {
                    groups.Names.Add(new string(split));
                }
            }
        }

        return groups;
    }

    /// <summary>
    /// Writes the groups value as a string
    /// </summary>
    /// <param name="value">Value to write</param>
    /// <returns>The Json serialization of <paramref name="value"/></returns>
    private static string WriteInternal(Groups value) => string.Join(", ", value.Names);
    #endregion
}
