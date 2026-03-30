using System.Threading.Tasks;
using Xunit;

namespace Mithril.Analyzers.Tests;

public sealed class MethodTooLongAnalyzerTests
{
    [Fact]
    public async Task Reports_When_Method_Exceeds_Configured_Limit()
    {
        var source = """
            namespace Sample;

            class Example
            {
                void [|TooLong|]()
                {
                    var one = 1;
                    var two = 2;
                    var three = one + two;
                }
            }
            """;

        await AnalyzerVerifier<MethodTooLongAnalyzer>.VerifyAnalyzerAsync(
            source,
            "BGA002");
    }

    [Fact]
    public async Task Ignores_ExpressionBodied_Methods()
    {
        var source = """
            namespace Sample;

            class Example
            {
                int Size() => 42;
            }
            """;

        await AnalyzerVerifier<MethodTooLongAnalyzer>.VerifyAnalyzerAsync(source);
    }
}
