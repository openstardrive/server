using System;
using System.Linq.Expressions;
using Moq;
using Moq.Language.Flow;
using OpenStardriveServer.Domain;

namespace OpenStardriveServer.UnitTests;

public static class MoqExtensions
{
    public static void VerifyNever<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression)
        where T : class
    {
        mock.Verify(expression, Times.Never);
    }

    public static void VerifyNever<T>(this Mock<T> mock, Expression<Action<T>> expression)
        where T : class
    {
        mock.Verify(expression, Times.Never);
    }

    public static void ReturnsNone<T, TResult>(this ISetup<T, Maybe<TResult>> setup) where T : class
    {
        setup.Returns(Maybe<TResult>.None);
    }
    
    public static void ReturnsSome<T, TResult>(this ISetup<T, Maybe<TResult>> setup, TResult result) where T : class
    {
        setup.Returns(Maybe<TResult>.Some(result));
    }
}