using System;
using System.Collections.Generic;

namespace WatchStore.Api.Extensions;

public static class LinqHelperExtension
{
    public static T MaxOrElse<T>(this IEnumerable<T> source, Func<T> fallbackFactory)
        where T : notnull, IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(fallbackFactory);

        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return fallbackFactory();
        }

        var max = enumerator.Current;

        var comparer = Comparer<T>.Default;
        while (enumerator.MoveNext())
        {
            if (comparer.Compare(enumerator.Current, max) > 0)
            {
                max = enumerator.Current;
            }
        }

        return max;
    }

    public static T MaxOrElse<T>(this IEnumerable<T> source, T fallbackValue)
        where T : notnull, IComparable<T>
    {
        return source.MaxOrElse(() => fallbackValue);
    }
}
