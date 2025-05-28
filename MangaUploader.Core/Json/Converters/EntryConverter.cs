using System.Text.Json;
using System.Text.Json.Serialization;
using MangaUploader.Core.Extensions.Strings;
using MangaUploader.Core.Models.Cubari;

namespace MangaUploader.Core.Json.Converters;

/// <summary>
/// Chapter entry converter
/// </summary>
public sealed class EntryConverter : JsonConverter<Entry>
{
    #region Overrides
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeof(Entry).IsAssignableFrom(typeToConvert);

    /// <inheritdoc />
    public override Entry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType switch
    {
        JsonTokenType.String     => ParseProxyEntry(ref reader),
        JsonTokenType.StartArray => ParseListEntry(ref reader),
        _                        => throw new JsonException($"Unknown Entry JSON token ({reader.TokenType}).")
    };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Entry value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case ProxyEntry proxyEntry:
                WriteProxyEntry(writer, proxyEntry);
                break;

            case ListEntry listEntry:
                WriteListEntry(writer, listEntry);
                break;

            default:
                throw new JsonException($"Unknown entry type ({value.GetType().Name})");
        }
    }
    #endregion

    #region Static Methods
    /// <summary>
    /// Parses a proxy entry value
    /// </summary>
    /// <param name="reader">JSON reader</param>
    /// <returns>The parsed proxy entry</returns>
    /// <exception cref="JsonException">If the entry could not be parsed</exception>
    private static ProxyEntry ParseProxyEntry(ref Utf8JsonReader reader)
    {
        string? value = reader.GetString();
        ProxyEntry? entry = ProxyEntry.FromUriString(value);
        return entry ?? throw new JsonException($"Invalid proxy entry Uri detected ({value}).");
    }

    /// <summary>
    /// Parses a list entry value
    /// </summary>
    /// <param name="reader">JSON reader</param>
    /// <returns>The parsed list entry</returns>
    /// <exception cref="JsonException">If the entry could not be parsed</exception>
    private static ListEntry ParseListEntry(ref Utf8JsonReader reader)
    {
        reader.Read();
        ListEntry entry = new();
        while (reader.TokenType is not JsonTokenType.EndArray)
        {
            if (reader.TokenType is not JsonTokenType.String) throw new JsonException($"Unknown list entry element JSON token ({reader.TokenType}).");

            string? value = reader.GetString();
            if (string.IsNullOrEmpty(value)) throw new JsonException("Empty list entry element.");

            reader.Read();
            entry.Images.Add(value.AsUri());
        }

        return entry;
    }

    /// <summary>
    /// Writes a proxy entry value
    /// </summary>
    /// <param name="writer">JSON writer</param>
    /// <param name="proxyEntry">The proxy entry to write</param>
    private static void WriteProxyEntry(Utf8JsonWriter writer, ProxyEntry proxyEntry) => writer.WriteStringValue(proxyEntry.ToUriString());

    /// <summary>
    /// Writes a list entry value
    /// </summary>
    /// <param name="writer">JSON writer</param>
    /// <param name="listEntry">the list entry to write</param>
    private static void WriteListEntry(Utf8JsonWriter writer, ListEntry listEntry)
    {
        writer.WriteStartArray();
        foreach (Uri uri in listEntry.Images)
        {
            writer.WriteStringValue(uri.ToString());
        }
        writer.WriteEndArray();
    }
    #endregion
}
