using System;

// ReSharper disable once CheckNamespace
namespace MangaUploader.Core.Extensions.Strings;

/// <summary>
/// String extensions
/// </summary>
public static class StringExtensions
{
    #region Extension Methods
    /// <summary>
    /// Converts the string value to a Uri
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <returns>The Uri value</returns>
    public static Uri AsUri(this string value) => new(value);
    #endregion
}
