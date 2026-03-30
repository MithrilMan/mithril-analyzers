using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Mithril.Analyzers.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mithril.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferTryGetValueAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(DiagnosticDescriptors.PreferTryGetValue);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IfStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (GeneratedCodeDetector.IsGenerated(context.Node.SyntaxTree, context.CancellationToken))
        {
            return;
        }

        var ifStatement = (IfStatementSyntax)context.Node;
        if (ifStatement.Else is not null)
        {
            return;
        }

        if (!TryGetContainsKeyCondition(
                UnwrapParentheses(ifStatement.Condition),
                context.SemanticModel,
                context.CancellationToken,
                out var dictionaryExpression,
                out var keyExpression,
                out var diagnosticLocation))
        {
            return;
        }

        if (!BodyContainsMatchingIndexer(ifStatement.Statement, dictionaryExpression, keyExpression))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.PreferTryGetValue, diagnosticLocation));
    }

    private static bool TryGetContainsKeyCondition(
        ExpressionSyntax condition,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        out ExpressionSyntax dictionaryExpression,
        out ExpressionSyntax keyExpression,
        out Location diagnosticLocation)
    {
        dictionaryExpression = null!;
        keyExpression = null!;
        diagnosticLocation = Location.None;

        if (condition is not InvocationExpressionSyntax invocationExpression ||
            invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccess ||
            memberAccess.Name.Identifier.ValueText != "ContainsKey" ||
            invocationExpression.ArgumentList.Arguments.Count != 1)
        {
            return false;
        }

        if (semanticModel.GetSymbolInfo(invocationExpression, cancellationToken).Symbol is not IMethodSymbol methodSymbol ||
            !IsDictionaryLike(methodSymbol.ContainingType))
        {
            return false;
        }

        dictionaryExpression = memberAccess.Expression;
        keyExpression = invocationExpression.ArgumentList.Arguments[0].Expression;
        diagnosticLocation = memberAccess.Name.GetLocation();
        return true;
    }

    private static bool IsDictionaryLike(INamedTypeSymbol? typeSymbol)
    {
        if (typeSymbol is null)
        {
            return false;
        }

        return ImplementsInterface(typeSymbol, "System.Collections.Generic.IDictionary<TKey, TValue>") ||
               ImplementsInterface(typeSymbol, "System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>");
    }

    private static bool ImplementsInterface(INamedTypeSymbol typeSymbol, string interfaceDisplayName)
    {
        if (typeSymbol.OriginalDefinition.ToDisplayString() == interfaceDisplayName)
        {
            return true;
        }

        return typeSymbol.AllInterfaces.Any(
            interfaceType => interfaceType.OriginalDefinition.ToDisplayString() == interfaceDisplayName);
    }

    private static bool BodyContainsMatchingIndexer(
        StatementSyntax statement,
        ExpressionSyntax dictionaryExpression,
        ExpressionSyntax keyExpression)
    {
        return statement
            .DescendantNodesAndSelf(ShouldDescendIntoNode)
            .OfType<ElementAccessExpressionSyntax>()
            .Any(elementAccess =>
                elementAccess.ArgumentList.Arguments.Count == 1 &&
                SyntaxFactory.AreEquivalent(elementAccess.Expression, dictionaryExpression) &&
                SyntaxFactory.AreEquivalent(elementAccess.ArgumentList.Arguments[0].Expression, keyExpression));
    }

    private static bool ShouldDescendIntoNode(SyntaxNode node)
        => node is not AnonymousFunctionExpressionSyntax &&
           node is not LocalFunctionStatementSyntax &&
           node is not TypeDeclarationSyntax;

    private static ExpressionSyntax UnwrapParentheses(ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            expression = parenthesizedExpression.Expression;
        }

        return expression;
    }
}
