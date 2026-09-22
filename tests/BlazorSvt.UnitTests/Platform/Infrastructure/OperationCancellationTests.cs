using System.Data.SqlClient;
using System.Reflection;
using BlazorSvt.Platform.Infrastructure;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Infrastructure;

[Trait("Category", "Unit")]
public class OperationCancellationTests
{
    public static IEnumerable<object[]> SqlCancellationCases()
    {
        yield return [-2, "Timeout expired"];
        yield return [50000, "Operation cancelled by user"];
        yield return [50000, "Operation canceled by user"];
    }

    [Fact]
    public void WhenOperationCanceledAndTokenCannotBeCanceled_ReturnsTrue()
    {
        OperationCancellation.IsCancellation(new OperationCanceledException()).Should().BeTrue();
    }

    [Fact]
    public void WhenOperationCanceledAndTokenIsNotCanceled_ReturnsFalse()
    {
        using var cts = new CancellationTokenSource();

        OperationCancellation.IsCancellation(new OperationCanceledException(), cts.Token).Should().BeFalse();
    }

    [Fact]
    public void WhenOperationCanceledAndTokenIsCanceled_ReturnsTrue()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        OperationCancellation.IsCancellation(new OperationCanceledException(), cts.Token).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SqlCancellationCases))]
    public void WhenSqlCancellationAndTokenCannotBeCanceled_ReturnsTrue(int number, string message)
    {
        var exception = CreateSqlException(number, message);

        OperationCancellation.IsCancellation(exception).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SqlCancellationCases))]
    public void WhenSqlCancellationAndTokenIsCanceled_ReturnsTrue(int number, string message)
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        OperationCancellation.IsCancellation(CreateSqlException(number, message), cts.Token).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SqlCancellationCases))]
    public void WhenSqlCancellationAndTokenIsNotCanceled_ReturnsFalse(int number, string message)
    {
        using var cts = new CancellationTokenSource();

        OperationCancellation.IsCancellation(CreateSqlException(number, message), cts.Token).Should().BeFalse();
    }

    [Fact]
    public void WhenSqlExceptionIsUnrelated_ReturnsFalse()
    {
        var exception = CreateSqlException(1205, "Transaction was deadlocked");

        OperationCancellation.IsCancellation(exception).Should().BeFalse();

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        OperationCancellation.IsCancellation(exception, cts.Token).Should().BeFalse();
    }

    [Fact]
    public void WhenExceptionIsNotCancellation_ReturnsFalse()
    {
        OperationCancellation.IsCancellation(new InvalidOperationException("cancelled")).Should().BeFalse();
    }

#pragma warning disable CS0618 // System.Data.SqlClient — как в OperationCancellation
    private static SqlException CreateSqlException(int number, string message)
    {
        var error = CreateSqlError(number, message);
        var errors = Activator.CreateInstance(typeof(SqlErrorCollection), nonPublic: true)
            ?? throw new InvalidOperationException("SqlErrorCollection has no constructor.");
        typeof(SqlErrorCollection)
            .GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(errors, [error]);

        var create = typeof(SqlException)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Single(method =>
            {
                if (method.Name != "CreateException")
                    return false;

                var parameters = method.GetParameters();
                return parameters.Length == 2
                    && parameters[0].ParameterType == typeof(SqlErrorCollection)
                    && parameters[1].ParameterType == typeof(string);
            });

        var exception = (SqlException)create.Invoke(null, [errors, "1.0.0"])!;
        exception.Number.Should().Be(number);
        exception.Message.Should().Contain(message);
        return exception;
    }

    private static object CreateSqlError(int number, string message)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var ctor = typeof(SqlError).GetConstructors(flags)
            .Where(candidate =>
            {
                var parameters = candidate.GetParameters();
                return parameters.Length >= 7
                    && parameters[0].ParameterType == typeof(int)
                    && parameters[1].ParameterType == typeof(byte)
                    && parameters[2].ParameterType == typeof(byte)
                    && parameters[3].ParameterType == typeof(string)
                    && parameters[4].ParameterType == typeof(string)
                    && parameters[5].ParameterType == typeof(string)
                    && parameters[6].ParameterType == typeof(int);
            })
            .OrderBy(candidate => candidate.GetParameters().Length)
            .First();

        var args = new object?[ctor.GetParameters().Length];
        args[0] = number;
        args[1] = (byte)0;
        args[2] = (byte)0;
        args[3] = "server";
        args[4] = message;
        args[5] = "proc";
        args[6] = 0;
        for (var i = 7; i < args.Length; i++)
        {
            var type = ctor.GetParameters()[i].ParameterType;
            args[i] = type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        return ctor.Invoke(args);
    }
#pragma warning restore CS0618
}
