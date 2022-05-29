using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace VelaraUtils.Utils;

internal class DynamicDictionary<T> : DynamicObject
{
    private readonly Dictionary<string, dynamic?> _dictionary = new();

    private delegate object? ConversionDelegate(DynamicDictionary<T> dict);
    private readonly Dictionary<Type, ConversionDelegate> _conversionTable = new()
    {
        { typeof(IDictionary<string, dynamic?>), dict => new Dictionary<string, dynamic?>(dict._dictionary.AsEnumerable()) },
        { typeof(IEnumerable<dynamic?>), dict => dict._dictionary.Values }
    };

    public DynamicDictionary(IEnumerable<T> source, string remainingName, params string[] propertyNames)
    {
        List<T> remaining = _dictionary[remainingName] = new List<T>();

        using IEnumerator<T> sourceEnumator = source.GetEnumerator();
        using IEnumerator<string> propEnumator = propertyNames.AsEnumerable().GetEnumerator();
        while (sourceEnumator.MoveNext())
        {
            if (propEnumator.MoveNext())
            {
                _dictionary[propEnumator.Current] = sourceEnumator.Current;
                continue;
            }

            remaining.Add(sourceEnumator.Current);
        }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (!_dictionary.ContainsKey(binder.Name))
        {
            result = null;
            return false;
        }

        result = _dictionary[binder.Name];
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        _dictionary[binder.Name] = value;
        return true;
    }

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        Type? type = _conversionTable.Keys.FirstOrDefault(k => binder.Type.IsAssignableTo(k));
        if (type is null)
            return base.TryConvert(binder, out result);

        result = _conversionTable[type](this);
        return true;
    }
}

public static class CollectionExtensions
{
    public static void For<T>(this IEnumerable<T> source, int count, Action<T, int> action)
    {
        using IEnumerator<T> enumerator = source.GetEnumerator();
        for (int i = 0; i < count; i++)
        {
            if (!enumerator.MoveNext()) break;
            action(enumerator.Current, i);
        }
    }

    public static void For<T>(this IEnumerable<T> source, int count, Action<T> action)
    {
        source.For(count, (o, _) => action(o));
    }

    public static void For<T>(this ICollection<T> source, Action<T> action)
    {
        source.For(source.Count, (o, _) => action(o));
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        int i = 0;
        using IEnumerator<T> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            action(enumerator.Current, i);
            i++;
        }
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        source.ForEach((o, _) => action(o));
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> source, float delayMs, Action<T, int> action)
    {
        int delay = (int)MathF.Floor(delayMs);

        int i = 0;
        using IEnumerator<T> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            action(enumerator.Current, i);
            i++;

            if (delay < 1) continue;
            await Task.Delay(delay);
        }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> source, float delay, Action<T> action) =>
        await source.ForEachAsync(delay, (o, _) => action(o));

    public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, int count, Func<TSource, int, TResult> selector)
    {
        List<TResult> result = new();
        source.For(count, (o, i) => result.Add(selector(o, i)));
        return result;
    }

    public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, int count, Func<TSource, TResult> selector)
    {
        return source.Select(count, (o, _) => selector(o));
    }

    public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, int count, Func<TResult> selector)
    {
        return source.Select(count, (_, _) => selector());
    }

    public static TCollection Clear<TSource, TCollection>(this TCollection source, out IEnumerable<TSource> elements)
        where TCollection : ICollection<TSource>
    {
        elements = source.ToList();
        source.Clear();
        return source;
    }

    public static TCollection Add<TSource, TCollection>(this TCollection source, TSource element)
        where TCollection : ICollection<TSource>
    {
        source.Add(element);
        return source;
    }

    public static TCollection AddRange<TSource, TCollection>(this TCollection source, IEnumerable<TSource> elements)
        where TCollection : ICollection<TSource>
    {
        elements.ToList().ForEach(source.Add);
        return source;
    }

    public static TCollection AddRange<TSource, TCollection>(this TCollection source, params TSource[] elements)
        where TCollection : ICollection<TSource>
    {
        elements.For(source.Add);
        return source;
    }

    public static T? Remove<T>(this ICollection<T> source, T? element)
    {
        if (element is null || !source.Contains(element))
            return default;

        source.Remove(element);
        return element;
    }

    public static T Pop<T>(this ICollection<T> source)
    {
        return source.Remove<T>(source.First())!;
    }

    public static IEnumerable<T?> Pop<T>(this ICollection<T> source, int count)
    {
        return source.Select(count, source.Pop).ToList();
    }

    public static T? PopOrDefault<T>(this ICollection<T> source)
    {
        return source.Remove<T>(source.FirstOrDefault());
    }

    public static IEnumerable<T?> PopOrDefault<T>(this ICollection<T> source, int count)
    {
        return source.Select(count, source.PopOrDefault).ToList();
    }

    public static T PopBack<T>(this ICollection<T> source)
    {
        return source.Remove<T>(source.Last())!;
    }

    public static T? PopBackOrDefault<T>(this ICollection<T> source)
    {
        return source.Remove<T>(source.LastOrDefault());
    }

    public static int PushFront<TSource, TCollection>(this TCollection source, TSource element)
        where TCollection : ICollection<TSource>
    {
        return source
            .Clear(out IEnumerable<TSource> elements)
            .Add<TSource, TCollection>(element)
            .AddRange(elements)
            .Count;
    }

    public static dynamic Parse<T>(this IEnumerable<T> source, string remainingName, params string[] propertyNames)
    {
        return new DynamicDictionary<T>(source, remainingName, propertyNames);
    }

    public static string Join(this string source, string separator, params string[] values)
    {
        return values.Length < 1 ?
            string.Join(separator, values.Prepend(source)) :
            source;
    }

    public static string Join(this IEnumerable<string> source, string separator = " ")
    {
        return string.Join(separator, source);
    }
}
