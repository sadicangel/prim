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
    public virtual string FullyQualifiedName => field ??= $"{ContainingModule.FullyQualifiedName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";

    public virtual string FullName => field ??= ContainingModule.IsGlobal ? Name : $"{ContainingModule.FullName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";

    public bool IsPredefined { get; init; }

    public sealed override string ToString() => $"{Name}: {Type.Name}";
}
