using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class LabelSymbol(
    SyntaxNode Syntax,
    string Name,
    ScopeSymbol ContainingScope)
    : Symbol(
        BoundKind.LabelSymbol,
        Syntax,
        Name,
        ContainingScope.Never,
        ContainingScope.Module,
        ContainingScope,
        IsStatic: true,
        IsReadOnly: true)
{
    public LabelSymbol(string name, ModuleSymbol containingModule)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, containingModule)
    {
    }

    public override IEnumerable<Symbol> DeclaredSymbols => [];
}
