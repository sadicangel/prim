using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class LabelSymbol(SyntaxNode Syntax, string Name, TypeSymbol Type, ModuleSymbol ContainingModule)
    : Symbol(BoundKind.LabelSymbol, Syntax, Name, Type, ContainingModule, IsStatic: true, IsReadOnly: true)
{
    public LabelSymbol(string name, TypeSymbol runtimeType, ModuleSymbol containingModule)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, runtimeType, containingModule)
    {
    }

    public override IEnumerable<Symbol> DeclaredSymbols => [];
}
