using System.Collections.Immutable;
using Mithril.Analyzers.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mithril.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AccessorTooLongAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(DiagnosticDescriptors.AccessorTooLong);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.GetAccessorDeclaration,
            SyntaxKind.SetAccessorDeclaration,
            SyntaxKind.InitAccessorDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (GeneratedCodeDetector.IsGenerated(context.Node.SyntaxTree, context.CancellationToken))
        {
            return;
        }

        var accessorDeclaration = (AccessorDeclarationSyntax)context.Node;
        if (accessorDeclaration.ExpressionBody is not null || accessorDeclaration.Body is null)
        {
            return;
        }

        var maxLines = AnalyzerOptionReader.GetMaxAccessorLines(context.Options, accessorDeclaration.SyntaxTree);
        var lineCount = LineCounter.CountLines(accessorDeclaration.SyntaxTree, accessorDeclaration.Body);
        if (lineCount <= maxLines)
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptors.AccessorTooLong,
                accessorDeclaration.Keyword.GetLocation(),
                accessorDeclaration.Keyword.ValueText,
                lineCount,
                maxLines));
    }
}
