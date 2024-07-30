using System.Runtime.CompilerServices;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundTree(
    SyntaxTree SyntaxTree,
    IBoundScope BoundScope,
    BoundCompilationUnit CompilationUnit,
    DiagnosticBag Diagnostics)
    : IEquatable<BoundTree>
{
    private BoundTree(SyntaxTree syntaxTree, IBoundScope boundScope)
        : this(syntaxTree, boundScope, CompilationUnit: null!, Diagnostics: [])
    {
        CompilationUnit = Binder.Bind(this, BoundScope);
    }

    public bool Equals(BoundTree? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static BoundTree Bind(SyntaxTree syntaxTree, IBoundScope boundScope) =>
        new(syntaxTree, boundScope);
}
