using System.Threading.Tasks;
using Xunit;

namespace Mithril.Analyzers.Tests;

public sealed class AsyncMethodShouldExposeCancellationAnalyzerTests
{
    [Fact]
    public async Task Reports_When_Task_Method_Has_No_Cancellation_Path()
    {
        var source = """
            using System.Threading.Tasks;

            namespace Sample;

            class Example
            {
                public async Task [|ExecuteAsync|]()
                {
                    await Task.Delay(1);
                }
            }
            """;

        await AnalyzerVerifier<AsyncMethodShouldExposeCancellationAnalyzer>.VerifyAnalyzerAsync(
            source,
            "BGA004");
    }

    [Fact]
    public async Task Does_Not_Report_When_Method_Exposes_CancellationToken()
    {
        var source = """
            using System.Threading;
            using System.Threading.Tasks;

            namespace Sample;

            class Example
            {
                public async Task ExecuteAsync(CancellationToken cancellationToken)
                {
                    await Task.Delay(1, cancellationToken);
                }
            }
            """;

        await AnalyzerVerifier<AsyncMethodShouldExposeCancellationAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task Does_Not_Report_When_Parameter_Contains_Public_CancellationToken_Property()
    {
        var source = """
            using System.Threading;
            using System.Threading.Tasks;

            namespace Sample;

            sealed class RequestContext
            {
                public CancellationToken CancellationToken { get; init; }
            }

            class Example
            {
                public Task ExecuteAsync(RequestContext context)
                {
                    return Task.CompletedTask;
                }
            }
            """;

        await AnalyzerVerifier<AsyncMethodShouldExposeCancellationAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task Does_Not_Report_For_Test_Methods_When_Config_Allows_It()
    {
        var source = """
            using System;
            using System.Threading.Tasks;

            namespace Sample;

            sealed class FactAttribute : Attribute
            {
            }

            class ExampleTests
            {
                [Fact]
                public async Task ExecuteAsync()
                {
                    await Task.Delay(1);
                }
            }
            """;

        await AnalyzerVerifier<AsyncMethodShouldExposeCancellationAnalyzer>.VerifyAnalyzerAsync(source);
    }
}
