using System.Runtime.CompilerServices;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundTree(
    SyntaxTree SyntaxTree,
    ScopeSymbol BoundScope,
    BoundCompilationUnit CompilationUnit,
    DiagnosticBag Diagnostics)
    : IEquatable<BoundTree>
{
    private BoundTree(SyntaxTree syntaxTree, ScopeSymbol boundScope)
        : this(syntaxTree, boundScope, CompilationUnit: null!, Diagnostics: [])
    {
        CompilationUnit = Binder.Bind(this, BoundScope);
    }

    public bool Equals(BoundTree? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static BoundTree Bind(SyntaxTree syntaxTree, ScopeSymbol boundScope) =>
        new(syntaxTree, boundScope);
}
