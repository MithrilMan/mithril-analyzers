using Microsoft.CodeAnalysis;

namespace Mithril.Analyzers.Common;

internal static class DiagnosticDescriptors
{
    private const string Category = "Maintainability";
    private const string DocsBaseUrl = "https://github.com/MithrilMan/mithril-analyzers/blob/main/docs/rules/";

    public static readonly DiagnosticDescriptor FileTooLong = Create(
        id: "BGA001",
        title: "File too long",
        messageFormat: "File has {0} lines, which exceeds the configured maximum of {1}",
        defaultSeverity: DiagnosticSeverity.Warning,
        description: "Flags oversized C# files before they turn into hard-to-maintain god files.",
        docsSlug: "BGA001-file-too-long");

    public static readonly DiagnosticDescriptor MethodTooLong = Create(
        id: "BGA002",
        title: "Method too long",
        messageFormat: "Method '{0}' has {1} lines, which exceeds the configured maximum of {2}",
        defaultSeverity: DiagnosticSeverity.Warning,
        description: "Flags long methods, constructors, and local functions that are turning into orchestration blobs.",
        docsSlug: "BGA002-method-too-long");

    public static readonly DiagnosticDescriptor AccessorTooLong = Create(
        id: "BGA003",
        title: "Accessor too long",
        messageFormat: "{0} accessor has {1} lines, which exceeds the configured maximum of {2}",
        defaultSeverity: DiagnosticSeverity.Warning,
        description: "Flags property accessors that hide too much workflow logic.",
        docsSlug: "BGA003-accessor-too-long");

    public static readonly DiagnosticDescriptor AsyncMethodShouldExposeCancellation = Create(
        id: "BGA004",
        title: "Async method should expose cancellation",
        messageFormat: "Async method '{0}' does not expose a cancellation path",
        defaultSeverity: DiagnosticSeverity.Info,
        description: "Encourages cancellable async flows in application and orchestration code.",
        docsSlug: "BGA004-async-method-should-expose-cancellation");

    public static readonly DiagnosticDescriptor PreferTryGetValue = Create(
        id: "BGA005",
        title: "Prefer TryGetValue",
        messageFormat: "Use 'TryGetValue' instead of 'ContainsKey' followed by an indexer lookup",
        defaultSeverity: DiagnosticSeverity.Info,
        description: "Avoids double dictionary lookups when a key existence check is followed by an indexer access.",
        docsSlug: "BGA005-prefer-try-get-value");

    private static DiagnosticDescriptor Create(
        string id,
        string title,
        string messageFormat,
        DiagnosticSeverity defaultSeverity,
        string description,
        string docsSlug)
        => new(
            id,
            title,
            messageFormat,
            Category,
            defaultSeverity,
            isEnabledByDefault: true,
            description: description,
            helpLinkUri: DocsBaseUrl + docsSlug + ".md");
}
