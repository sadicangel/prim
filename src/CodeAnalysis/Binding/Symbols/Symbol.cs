using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal abstract record class Symbol(BoundKind BoundKind, SyntaxNode Syntax, string Name)
    : BoundNode(BoundKind, Syntax)
{
    public abstract PrimType Type { get; }

    public override IEnumerable<BoundNode> Children() => [];
}
