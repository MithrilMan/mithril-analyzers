using System.Collections.Immutable;
using System.Linq;
using Mithril.Analyzers.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mithril.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FileTooLongAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(DiagnosticDescriptors.FileTooLong);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(Analyze);
    }

    private static void Analyze(SyntaxTreeAnalysisContext context)
    {
        if (GeneratedCodeDetector.IsGenerated(context.Tree, context.CancellationToken))
        {
            return;
        }

        var maxLines = AnalyzerOptionReader.GetMaxFileLines(context.Options, context.Tree);
        var lineCount = LineCounter.CountFileLines(context.Tree, context.CancellationToken);
        if (lineCount <= maxLines)
        {
            return;
        }

        var location = GetDiagnosticLocation(context.Tree.GetRoot(context.CancellationToken));
        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.FileTooLong, location, lineCount, maxLines));
    }

    private static Location GetDiagnosticLocation(SyntaxNode root)
    {
        var currentMember = root.DescendantNodes().OfType<MemberDeclarationSyntax>().FirstOrDefault();
        while (currentMember is BaseNamespaceDeclarationSyntax namespaceDeclaration && namespaceDeclaration.Members.Count > 0)
        {
            currentMember = namespaceDeclaration.Members[0];
        }

        return currentMember?.GetFirstToken(includeZeroWidth: true).GetLocation() ?? root.GetLocation();
    }
}
