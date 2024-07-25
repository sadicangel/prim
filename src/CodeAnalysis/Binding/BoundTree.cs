using System.Runtime.CompilerServices;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundTree(
    SyntaxTree SyntaxTree,
    BoundScope BoundScope,
    BoundCompilationUnit CompilationUnit,
    DiagnosticBag Diagnostics)
    : IEquatable<BoundTree>
{
    private BoundTree(SyntaxTree syntaxTree, BoundScope boundScope)
        : this(syntaxTree, boundScope, CompilationUnit: null!, Diagnostics: [])
    {
        CompilationUnit = Binder.Bind(this, BoundScope);
    }

    public bool Equals(BoundTree? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static BoundTree Bind(SyntaxTree syntaxTree, BoundScope boundScope) =>
        new(syntaxTree, boundScope);
}
