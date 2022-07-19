using System;
using System.Linq.Expressions;
using Moq;

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
}