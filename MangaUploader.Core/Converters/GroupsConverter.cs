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
        // Make sure we're reading a string
        if (reader.TokenType is not JsonTokenType.String) throw new JsonException("Expected string.");

        // Create empty Groups
        Groups groups = new();
        string? value = reader.GetString()?.Trim();
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

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Groups value, JsonSerializerOptions options)
    {
        // Get names string and write it
        string names = value.Names.Count switch
        {
            0 => string.Empty,
            1 => value.Names[0],
            _ => string.Join(", ", value.Names)
        };
        writer.WriteStringValue(names);
    }
    #endregion
}
