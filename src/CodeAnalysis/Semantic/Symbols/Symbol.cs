using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;
internal abstract record class Symbol(
    SymbolKind SymbolKind,
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    Symbol ContainingSymbol,
    ModuleSymbol ContainingModule,
    Modifiers Modifiers)
{
    public string FullName => field ??= Name is "<global>" ? Name : $"{ContainingModule.FullName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";

    public virtual bool Equals(Symbol? other) => other is not null && SymbolKind == other.SymbolKind && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, Name);

    public sealed override string ToString() => $"{Name}: {Type.Name}";
}
