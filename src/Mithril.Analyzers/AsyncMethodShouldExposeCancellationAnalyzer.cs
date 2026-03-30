using System.Collections.Immutable;
using System.Linq;
using Mithril.Analyzers.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mithril.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncMethodShouldExposeCancellationAnalyzer : DiagnosticAnalyzer
{
    private static readonly string[] TestAttributeNames =
    [
        "Fact",
        "Theory",
        "Test",
        "TestCase",
        "TestMethod",
        "DataTestMethod",
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(DiagnosticDescriptors.AsyncMethodShouldExposeCancellation);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.Method);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var methodSymbol = (IMethodSymbol)context.Symbol;
        if (methodSymbol.MethodKind != MethodKind.Ordinary)
        {
            return;
        }

        var sourceReference = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault();
        if (sourceReference is null)
        {
            return;
        }

        var syntax = sourceReference.GetSyntax(context.CancellationToken);
        if (GeneratedCodeDetector.IsGenerated(syntax.SyntaxTree, context.CancellationToken))
        {
            return;
        }

        if (!ReturnsTaskLike(methodSymbol.ReturnType) ||
            methodSymbol.IsOverride ||
            IsMain(methodSymbol) ||
            IsEventHandlerLike(methodSymbol) ||
            ImplementsInterfaceMember(methodSymbol) ||
            HasCancellationPath(methodSymbol))
        {
            return;
        }

        if (AnalyzerOptionReader.AllowMissingCancellationForTests(context.Options, syntax.SyntaxTree) &&
            IsTestMethod(methodSymbol))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptors.AsyncMethodShouldExposeCancellation,
                GetDiagnosticLocation(methodSymbol, syntax),
                methodSymbol.Name));
    }

    private static bool ReturnsTaskLike(ITypeSymbol returnType)
        => returnType is INamedTypeSymbol namedType &&
           namedType.Name is "Task" or "ValueTask" &&
           namedType.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks";

    private static bool HasCancellationPath(IMethodSymbol methodSymbol)
        => methodSymbol.Parameters.Any(parameter => IsCancellationToken(parameter.Type)) ||
           methodSymbol.Parameters.Any(parameter => HasPublicCancellationTokenProperty(parameter.Type));

    private static bool IsCancellationToken(ITypeSymbol typeSymbol)
        => typeSymbol.Name == "CancellationToken" &&
           typeSymbol.ContainingNamespace.ToDisplayString() == "System.Threading";

    private static bool HasPublicCancellationTokenProperty(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedType)
        {
            return false;
        }

        for (var current = namedType; current is not null; current = current.BaseType)
        {
            if (current.GetMembers()
                .OfType<IPropertySymbol>()
                .Any(property => !property.IsStatic &&
                                 property.DeclaredAccessibility == Accessibility.Public &&
                                 IsCancellationToken(property.Type)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsMain(IMethodSymbol methodSymbol)
        => methodSymbol.IsStatic && methodSymbol.Name == "Main";

    private static bool IsEventHandlerLike(IMethodSymbol methodSymbol)
        => methodSymbol.Parameters.Length == 2 &&
           methodSymbol.Parameters[0].Type.SpecialType == SpecialType.System_Object &&
           InheritsFromOrEquals(methodSymbol.Parameters[1].Type, "System.EventArgs");

    private static bool InheritsFromOrEquals(ITypeSymbol typeSymbol, string fullyQualifiedName)
    {
        for (var current = typeSymbol; current is not null; current = current.BaseType)
        {
            if (current.ToDisplayString() == fullyQualifiedName)
            {
                return true;
            }
        }

        return false;
    }

    private static bool ImplementsInterfaceMember(IMethodSymbol methodSymbol)
    {
        foreach (var interfaceType in methodSymbol.ContainingType.AllInterfaces)
        {
            foreach (var candidate in interfaceType.GetMembers(methodSymbol.Name).OfType<IMethodSymbol>())
            {
                var implementation = methodSymbol.ContainingType.FindImplementationForInterfaceMember(candidate);
                if (SymbolEqualityComparer.Default.Equals(implementation, methodSymbol))
                {
                    return true;
                }
            }
        }

        return methodSymbol.ExplicitInterfaceImplementations.Length > 0;
    }

    private static bool IsTestMethod(IMethodSymbol methodSymbol)
        => methodSymbol.GetAttributes().Any(attribute =>
        {
            var attributeName = attribute.AttributeClass?.Name;
            if (attributeName is null)
            {
                return false;
            }

            if (attributeName.EndsWith("Attribute", System.StringComparison.Ordinal))
            {
                attributeName = attributeName.Substring(0, attributeName.Length - 9);
            }

            return TestAttributeNames.Contains(attributeName, System.StringComparer.Ordinal);
        });

    private static Location GetDiagnosticLocation(IMethodSymbol methodSymbol, SyntaxNode syntax)
        => syntax is MethodDeclarationSyntax methodDeclaration
            ? methodDeclaration.Identifier.GetLocation()
            : methodSymbol.Locations.First(location => location.IsInSource);
}
