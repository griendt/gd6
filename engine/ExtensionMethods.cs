namespace engine;

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

    public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> collection)
    {
        var index = 0;
        foreach (var item in collection) {
            yield return (index, item);
            index++;
        }
    }

    public static T Tap<T>(this T item, Action<T> callback)
    {
        callback(item);
        return item;
    }
}