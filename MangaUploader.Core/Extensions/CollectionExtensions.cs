

// ReSharper disable once CheckNamespace
namespace MangaUploader.Core.Extensions.Collections;

/// <summary>
/// Collection extensions
/// </summary>
public static class CollectionExtensions
{
    #region Extension methods
    /// <summary>
    /// Checks if a collection is empty or not
    /// </summary>
    /// <param name="collection">Collection to check</param>
    /// <typeparam name="T">Item type in the collection</typeparam>
    /// <returns><see langword="true"/> if the collection is empty, otherwise <see langword="false"/></returns>
    public static bool IsEmpty<T>(this ICollection<T> collection) => collection.Count is 0;

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
    #endregion
}
