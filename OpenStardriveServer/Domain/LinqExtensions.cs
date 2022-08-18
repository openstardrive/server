using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain;

public static class LinqExtensions
{
    public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        return !source.Any(predicate);
    }
    
    public static IEnumerable<TSource> Replace<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> match, Func<TSource, TSource> replace)
    {
        return source.Select(x => match(x) ? replace(x) : x);
    }
    
    public static IEnumerable<TSource> Replace<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> match, Func<TSource, TSource> replace)
    {
        return source.Select((x, i) => match(x, i) ? replace(x) : x);
    }
}