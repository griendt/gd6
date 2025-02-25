namespace gd6;

public static class ExtensionMethods
{
    public static void SetOrAppend<TKey, TValue>(this IDictionary<TKey, List<TValue>> collection, TKey key, TValue value)
    {
        if (!collection.ContainsKey(key)) {
            collection[key] = [];
        }

        collection[key].Add(value);
    }

    public static void Each<T>(this IEnumerable<T> collection, Action<T> callback)
    {
        foreach (var item in collection) {
            callback(item);
        }
    }
}