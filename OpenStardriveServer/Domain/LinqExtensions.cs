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
}