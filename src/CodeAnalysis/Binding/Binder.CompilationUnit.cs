using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundCompilationUnit BindCompilationUnit(CompilationUnitSyntax syntax, BindingContext context)
    {
        var nodes = new BoundList<BoundNode>.Builder(syntax.SyntaxNodes.Count);
        foreach (var node in syntax.SyntaxNodes)
            nodes.Add(BindNode(node, context));

        return new BoundCompilationUnit(syntax, nodes.ToBoundList());
    }
}
