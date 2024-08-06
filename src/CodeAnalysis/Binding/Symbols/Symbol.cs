using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal abstract record class Symbol(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    ModuleSymbol ContainingModule,
    ScopeSymbol ContainingScope,
    bool IsStatic,
    bool IsReadOnly)
    : BoundNode(BoundKind, Syntax)
{
    public ScopeSymbol ContainingScope { get; init; } = ContainingScope;

    public QualifiedName QualifiedName { get; init; } = new QualifiedName(ContainingModule, Name);

    public abstract IEnumerable<Symbol> DeclaredSymbols { get; }

    public sealed override IEnumerable<Symbol> Children() => [];

    public virtual bool Equals(Symbol? other) => other is not null && BoundKind == other.BoundKind && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(BoundKind, Name);

    public sealed override string ToString() => $"{Name}: {Type.Name}";
}
