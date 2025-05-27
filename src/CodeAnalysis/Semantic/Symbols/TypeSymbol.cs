using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal abstract record class TypeSymbol(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    string Name,
    ModuleSymbol ContainingModule)
    : Symbol(BoundKind, Syntax, Name, ModuleSymbol.RuntimeType, ContainingModule, ContainingModule, Modifiers.Static | Modifiers.ReadOnly)
{
    protected List<Symbol> Members { get; } = [];

    public sealed override IEnumerable<Symbol> Children() => Members;

    public virtual bool Equals(TypeSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
