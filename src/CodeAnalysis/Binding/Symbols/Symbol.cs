using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;
internal abstract record class Symbol(BoundKind BoundKind, SyntaxNode Syntax, string Name)
    : BoundNode(BoundKind, Syntax)
{
    public abstract PrimType Type { get; }

    public override IEnumerable<BoundNode> Children() => [];

    public virtual bool Equals(Symbol? other) => other is not null && BoundKind == other.BoundKind && Name == other.Name;

    public override int GetHashCode() => HashCode.Combine(BoundKind, Name);

    public sealed override string ToString() => $"{Name}: {Type}";
}
