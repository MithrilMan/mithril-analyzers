using System.Collections.Immutable;
using Mithril.Analyzers.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mithril.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MethodTooLongAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(DiagnosticDescriptors.MethodTooLong);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.LocalFunctionStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (GeneratedCodeDetector.IsGenerated(context.Node.SyntaxTree, context.CancellationToken))
        {
            return;
        }

        if (!TryGetAnalyzedMember(context.Node, out var body, out var location, out var memberName))
        {
            return;
        }

        var maxLines = AnalyzerOptionReader.GetMaxMethodLines(context.Options, context.Node.SyntaxTree);
        var lineCount = LineCounter.CountLines(context.Node.SyntaxTree, body!);
        if (lineCount <= maxLines)
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(DiagnosticDescriptors.MethodTooLong, location, memberName, lineCount, maxLines));
    }

    private static bool TryGetAnalyzedMember(
        SyntaxNode node,
        out BlockSyntax? body,
        out Location location,
        out string memberName)
    {
        body = null;
        location = Location.None;
        memberName = string.Empty;

        switch (node)
        {
            case MethodDeclarationSyntax methodDeclaration
                when methodDeclaration.ExpressionBody is null && methodDeclaration.Body is not null:
                body = methodDeclaration.Body;
                location = methodDeclaration.Identifier.GetLocation();
                memberName = methodDeclaration.Identifier.ValueText;
                return true;

            case ConstructorDeclarationSyntax constructorDeclaration
                when constructorDeclaration.ExpressionBody is null && constructorDeclaration.Body is not null:
                body = constructorDeclaration.Body;
                location = constructorDeclaration.Identifier.GetLocation();
                memberName = constructorDeclaration.Identifier.ValueText;
                return true;

            case LocalFunctionStatementSyntax localFunctionStatement
                when localFunctionStatement.ExpressionBody is null && localFunctionStatement.Body is not null:
                body = localFunctionStatement.Body;
                location = localFunctionStatement.Identifier.GetLocation();
                memberName = localFunctionStatement.Identifier.ValueText;
                return true;

            default:
                return false;
        }
    }
}
