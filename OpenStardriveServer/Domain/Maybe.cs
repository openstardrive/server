using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain;
/*
 * Modified from: https://github.com/pluralsight/maybe-dotnet
 */

public struct Maybe<T>
{
    readonly IEnumerable<T> values;

    public static Maybe<T> Some(T value)
    {
        if (value == null)
        {
            throw new InvalidOperationException();
        }

        return new Maybe<T>(new[] { value });
    }

    public static Maybe<T> None => new Maybe<T>(new T[0]);

    private Maybe(IEnumerable<T> values)
    {
        this.values = values;
    }

    public bool HasValue => values != null && values.Any();

    public T Value
    {
        get
        {
            if (!HasValue)
            {
                throw new InvalidOperationException("Maybe does not have a value");
            }

            return values.Single();
        }
    }

    public T ValueOrDefault(T @default)
    {
        if (!HasValue)
        {
            return @default;
        }
        return values.Single();
    }

    public T ValueOrThrow(Exception e)
    {
        if (HasValue)
        {
            return Value;
        }
        throw e;
    }

    public U Case<U>(Func<T, U> some, Func<U> none)
    {
        return HasValue
            ? some(Value)
            : none();
    }

    public void Case(Action<T> some, Action none)
    {
        if (HasValue)
        {
            some(Value);
        }
        else
        {
            none();
        }
    }

    public void IfSome(Action<T> some)
    {
        if (HasValue)
        {
            some(Value);
        }
    }

    public Maybe<U> Map<U>(Func<T, Maybe<U>> map)
    {
        return HasValue
            ? map(Value)
            : Maybe<U>.None;
    }

    public Maybe<U> Map<U>(Func<T, U> map)
    {
        return HasValue
            ? Maybe.Some(map(Value))
            : Maybe<U>.None;
    }

    public Maybe<T> OrElse(Func<Maybe<T>> onElse)
    {
        return HasValue
            ? this
            : onElse();
    }
}

public static class Maybe
{
    public static Maybe<T> Some<T>(T value)
    {
        return Maybe<T>.Some(value);
    }
}
    
public static class MaybeExtensions
{
    public static Maybe<T> ToMaybe<T>(this T value) where T : class
    {
        return value != null
            ? Maybe.Some(value)
            : Maybe<T>.None;
    }

    public static Maybe<T> ToMaybe<T>(this T? nullable) where T : struct
    {
        return nullable.HasValue
            ? Maybe.Some(nullable.Value)
            : Maybe<T>.None;
    }

    public static Maybe<string> NoneIfEmpty(this string s)
    {
        return string.IsNullOrEmpty(s)
            ? Maybe<string>.None
            : Maybe.Some(s);
    }

    public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> self) where T : class
    {
        return self.FirstOrDefault().ToMaybe();
    }

    public static Maybe<T> FirstOrNone<T>(this IEnumerable<T?> self) where T : struct
    {
        return self.FirstOrDefault().ToMaybe();
    }

    public static Maybe<T> MaybeIf<T>(this bool self, T valueIfTrue)
    {
        return self ? Maybe.Some(valueIfTrue) : Maybe<T>.None;
    }
}