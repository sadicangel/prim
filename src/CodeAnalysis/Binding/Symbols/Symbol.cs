using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal abstract record class Symbol(BoundKind BoundKind, SyntaxNode SyntaxNode, string Name)
    : BoundNode(BoundKind, SyntaxNode)
{
    public abstract PrimType Type { get; }

    public override IEnumerable<BoundNode> Children() => [];
}
