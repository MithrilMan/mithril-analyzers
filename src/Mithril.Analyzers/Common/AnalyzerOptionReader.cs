using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mithril.Analyzers.Common;

internal static class AnalyzerOptionReader
{
    private const int DefaultMaxFileLines = 800;
    private const int DefaultMaxMethodLines = 100;
    private const int DefaultMaxAccessorLines = 60;
    private const bool DefaultAllowMissingCancellationForTests = true;

    public static int GetMaxFileLines(AnalyzerOptions options, SyntaxTree tree)
        => GetIntOption(options, tree, AnalyzerOptionNames.MaxFileLines, DefaultMaxFileLines);

    public static int GetMaxMethodLines(AnalyzerOptions options, SyntaxTree tree)
        => GetIntOption(options, tree, AnalyzerOptionNames.MaxMethodLines, DefaultMaxMethodLines);

    public static int GetMaxAccessorLines(AnalyzerOptions options, SyntaxTree tree)
        => GetIntOption(options, tree, AnalyzerOptionNames.MaxAccessorLines, DefaultMaxAccessorLines);

    public static bool AllowMissingCancellationForTests(AnalyzerOptions options, SyntaxTree tree)
        => GetBoolOption(
            options,
            tree,
            AnalyzerOptionNames.AllowMissingCancellationForTests,
            DefaultAllowMissingCancellationForTests);

    private static int GetIntOption(AnalyzerOptions options, SyntaxTree tree, string optionName, int fallback)
    {
        if (!TryGetOption(options, tree, optionName, out var rawValue))
        {
            return fallback;
        }

        return int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) && value > 0
            ? value
            : fallback;
    }

    private static bool GetBoolOption(AnalyzerOptions options, SyntaxTree tree, string optionName, bool fallback)
    {
        if (!TryGetOption(options, tree, optionName, out var rawValue))
        {
            return fallback;
        }

        return bool.TryParse(rawValue, out var value)
            ? value
            : fallback;
    }

    private static bool TryGetOption(AnalyzerOptions options, SyntaxTree tree, string optionName, out string rawValue)
    {
        rawValue = string.Empty;
        var found = options.AnalyzerConfigOptionsProvider.GetOptions(tree).TryGetValue(optionName, out var configuredValue);
        rawValue = configuredValue ?? string.Empty;
        return found && configuredValue is not null;
    }
}
