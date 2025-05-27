using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;
internal abstract record class Symbol(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    Symbol ContainingSymbol,
    ModuleSymbol ContainingModule,
    Modifiers Modifiers)
    : BoundNode(BoundKind, Syntax)
{
    public abstract override IEnumerable<Symbol> Children();

    public virtual bool Equals(Symbol? other) => other is not null && BoundKind == other.BoundKind && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(BoundKind, Name);

    public sealed override string ToString() => $"{Name}: {Type.Name}";
}
