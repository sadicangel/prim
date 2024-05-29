using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;
internal sealed class BoundTree
{
    public BoundTree(SyntaxTree syntaxTree, Func<BoundTree, BoundCompilationUnit> getRoot)
    {
        SyntaxTree = syntaxTree;
        Root = getRoot.Invoke(this);
    }

    public SyntaxTree SyntaxTree { get; }
    public BoundCompilationUnit Root { get; }
    public DiagnosticBag Diagnostics { get; init; } = [];

    public override string ToString() => $"BoundTree {{ Root = {Root} }}";

    public static BoundTree BindSymbols(SyntaxTree syntaxTree, BoundScope boundScope) =>
        new BoundTree(syntaxTree, (boundTree) => Binder.Bind(boundTree, boundScope));
}
