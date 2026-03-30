using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mithril.Analyzers.Common;

internal static class LineCounter
{
    public static int CountFileLines(SyntaxTree tree, CancellationToken cancellationToken)
        => tree.GetText(cancellationToken).Lines.Count;

    public static int CountLines(SyntaxTree tree, SyntaxNode node)
        => CountLines(tree, node.Span);

    public static int CountLines(SyntaxTree tree, TextSpan span)
    {
        var lineSpan = tree.GetLineSpan(span);
        return lineSpan.EndLinePosition.Line - lineSpan.StartLinePosition.Line + 1;
    }
}
