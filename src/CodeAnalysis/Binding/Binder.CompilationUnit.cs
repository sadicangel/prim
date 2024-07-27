using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundCompilationUnit BindCompilationUnit(CompilationUnitSyntax syntax, Context context)
    {
        var builder = ImmutableArray.CreateBuilder<BoundNode>(syntax.SyntaxNodes.Count);
        foreach (var node in syntax.SyntaxNodes)
            builder.Add(BindNode(node, context));
        var nodesNodes = new BoundList<BoundNode>(builder.ToImmutable());

        return new BoundCompilationUnit(syntax, nodesNodes);
    }
}
