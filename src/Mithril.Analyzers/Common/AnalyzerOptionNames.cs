namespace Mithril.Analyzers.Common;

internal static class AnalyzerOptionNames
{
    private const string Prefix = "mithril_analyzers";

    public const string MaxFileLines = Prefix + ".max_file_lines";
    public const string MaxMethodLines = Prefix + ".max_method_lines";
    public const string MaxAccessorLines = Prefix + ".max_accessor_lines";
    public const string AllowMissingCancellationForTests = Prefix + ".allow_missing_cancellation_for_tests";
}
