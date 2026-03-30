using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Mithril.Analyzers.Tests;

internal static class AnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    private const string DefaultEditorConfig = """
        root = true

        [*.cs]
        mithril_analyzers.max_file_lines = 5
        mithril_analyzers.max_method_lines = 3
        mithril_analyzers.max_accessor_lines = 2
        mithril_analyzers.allow_missing_cancellation_for_tests = true
        """;

    private static readonly ImmutableArray<MetadataReference> MetadataReferences = GetMetadataReferences();

    public static async Task VerifyAnalyzerAsync(string source, params string[] expectedDiagnosticIds)
        => await VerifyAnalyzerAsync(source, DefaultEditorConfig, "/Test0.cs", expectedDiagnosticIds);

    public static async Task VerifyAnalyzerAsync(
        string source,
        string editorConfig,
        string filePath,
        params string[] expectedDiagnosticIds)
    {
        var parseResult = ParseMarkup(source);
        Assert.Equal(expectedDiagnosticIds.Length, parseResult.Markers.Length);

        var syntaxTree = CSharpSyntaxTree.ParseText(
            parseResult.Source,
            new CSharpParseOptions(LanguageVersion.Latest),
            path: filePath);

        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: [syntaxTree],
            references: MetadataReferences,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilerErrors = compilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToImmutableArray();

        Assert.True(
            compilerErrors.Length == 0,
            "Compilation failed:\n" + string.Join(Environment.NewLine, compilerErrors.Select(d => d.ToString())));

        var analyzerOptions = new AnalyzerOptions(
            ImmutableArray<AdditionalText>.Empty,
            new TestAnalyzerConfigOptionsProvider(ParseEditorConfig(editorConfig)));

        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(new TAnalyzer()),
            new CompilationWithAnalyzersOptions(
                options: analyzerOptions,
                onAnalyzerException: null,
                concurrentAnalysis: true,
                logAnalyzerExecutionTime: false,
                reportSuppressedDiagnostics: false));

        var diagnostics = (await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync())
            .OrderBy(diagnostic => diagnostic.Location.SourceSpan.Start)
            .ToImmutableArray();

        Assert.Equal(expectedDiagnosticIds.Length, diagnostics.Length);

        for (var index = 0; index < expectedDiagnosticIds.Length; index++)
        {
            Assert.Equal(expectedDiagnosticIds[index], diagnostics[index].Id);

            var lineSpan = diagnostics[index].Location.GetLineSpan();
            Assert.Equal(parseResult.Markers[index].Line, lineSpan.StartLinePosition.Line + 1);
            Assert.Equal(parseResult.Markers[index].Column, lineSpan.StartLinePosition.Character + 1);
        }
    }

    private static ImmutableArray<MetadataReference> GetMetadataReferences()
    {
        var trustedPlatformAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))?
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries) ??
            [];

        return trustedPlatformAssemblies
            .Select(path => MetadataReference.CreateFromFile(path))
            .Cast<MetadataReference>()
            .ToImmutableArray();
    }

    private static ImmutableDictionary<string, string> ParseEditorConfig(string editorConfig)
    {
        var values = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in editorConfig.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#') || trimmed.StartsWith('['))
            {
                continue;
            }

            var separatorIndex = trimmed.IndexOf('=');
            if (separatorIndex < 0)
            {
                continue;
            }

            var key = trimmed[..separatorIndex].Trim();
            var value = trimmed[(separatorIndex + 1)..].Trim();
            values[key] = value;
        }

        return values.ToImmutable();
    }

    private static ParseResult ParseMarkup(string source)
    {
        var markers = ImmutableArray.CreateBuilder<int>();
        var builder = new StringBuilder(source.Length);

        for (var index = 0; index < source.Length; index++)
        {
            if (index + 1 < source.Length && source[index] == '[' && source[index + 1] == '|')
            {
                markers.Add(builder.Length);
                index++;
                continue;
            }

            if (index + 1 < source.Length && source[index] == '|' && source[index + 1] == ']')
            {
                index++;
                continue;
            }

            builder.Append(source[index]);
        }

        var cleanedSource = builder.ToString();
        return new ParseResult(
            cleanedSource,
            markers
                .Select(position => ToLineAndColumn(cleanedSource, position))
                .ToImmutableArray());
    }

    private static DiagnosticMarker ToLineAndColumn(string text, int position)
    {
        var line = 1;
        var column = 1;

        for (var index = 0; index < position; index++)
        {
            if (text[index] == '\n')
            {
                line++;
                column = 1;
            }
            else if (text[index] != '\r')
            {
                column++;
            }
        }

        return new DiagnosticMarker(line, column);
    }

    private sealed record ParseResult(string Source, ImmutableArray<DiagnosticMarker> Markers);

    private sealed record DiagnosticMarker(int Line, int Column);

    private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly TestAnalyzerConfigOptions _options;

        public TestAnalyzerConfigOptionsProvider(ImmutableDictionary<string, string> values)
        {
            _options = new TestAnalyzerConfigOptions(values);
        }

        public override AnalyzerConfigOptions GlobalOptions => _options;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            => _options;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            => _options;
    }

    private sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly ImmutableDictionary<string, string> _values;

        public TestAnalyzerConfigOptions(ImmutableDictionary<string, string> values)
        {
            _values = values;
        }

        public override bool TryGetValue(string key, out string value)
            => _values.TryGetValue(key, out value!);
    }
}
