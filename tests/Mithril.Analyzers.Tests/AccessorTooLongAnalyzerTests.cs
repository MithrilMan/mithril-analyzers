using System.Threading.Tasks;
using Xunit;

namespace Mithril.Analyzers.Tests;

public sealed class AccessorTooLongAnalyzerTests
{
    [Fact]
    public async Task Reports_When_Accessor_Exceeds_Configured_Limit()
    {
        var source = """
            namespace Sample;

            class Example
            {
                int Value
                {
                    [|get|]
                    {
                        var one = 1;
                        return one;
                    }
                }
            }
            """;

        await AnalyzerVerifier<AccessorTooLongAnalyzer>.VerifyAnalyzerAsync(
            source,
            "BGA003");
    }
}
