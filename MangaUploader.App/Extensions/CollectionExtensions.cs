using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace MangaUploader.Extensions.Collections;

/// <summary>
/// Collection extensions
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Adds a set of item to a collection
    /// </summary>
    /// <param name="collection">Collection to add to</param>
    /// <param name="items">Items to add</param>
    /// <typeparam name="T">Item type in the collection</typeparam>
    public static void AddRange<T>(this ICollection<T> collection, params ReadOnlySpan<T> items)
    {
        foreach (T item in items)
        {
            collection.Add(item);
        }
    }
}
