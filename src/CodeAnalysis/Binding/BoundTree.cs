using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;
internal sealed class BoundTree
{
    private BoundCompilationUnit? _compilationUnit;
    private BoundTree(SyntaxTree syntaxTree, BoundScope scope)
    {
        SyntaxTree = syntaxTree;
        Scope = scope;
    }

    public SyntaxTree SyntaxTree { get; }
    public BoundScope Scope { get; }
    public BoundCompilationUnit CompilationUnit { get => _compilationUnit ??= Binder.Bind(this, Scope); }
    public DiagnosticBag Diagnostics { get; init; } = [];

    public override string ToString() => $"BoundTree {{ CompilationUnit = {CompilationUnit} }}";

    public static BoundTree Bind(SyntaxTree syntaxTree, BoundScope boundScope) => new(syntaxTree, boundScope);
}
