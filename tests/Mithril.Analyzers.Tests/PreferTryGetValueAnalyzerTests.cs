using System.Threading.Tasks;
using Xunit;

namespace Mithril.Analyzers.Tests;

public sealed class PreferTryGetValueAnalyzerTests
{
    [Fact]
    public async Task Reports_When_ContainsKey_Is_Followed_By_Indexer_Lookup()
    {
        var source = """
            using System.Collections.Generic;

            namespace Sample;

            class Example
            {
                public int Read(Dictionary<string, int> values, string key)
                {
                    if (values.[|ContainsKey|](key))
                    {
                        return values[key];
                    }

                    return 0;
                }
            }
            """;

        await AnalyzerVerifier<PreferTryGetValueAnalyzer>.VerifyAnalyzerAsync(
            source,
            "BGA005");
    }

    [Fact]
    public async Task Does_Not_Report_For_Compound_Conditions()
    {
        var source = """
            using System.Collections.Generic;

            namespace Sample;

            class Example
            {
                public int Read(Dictionary<string, int> values, string key)
                {
                    if (values.ContainsKey(key) && key.Length > 0)
                    {
                        return values[key];
                    }

                    return 0;
                }
            }
            """;

        await AnalyzerVerifier<PreferTryGetValueAnalyzer>.VerifyAnalyzerAsync(source);
    }
}
