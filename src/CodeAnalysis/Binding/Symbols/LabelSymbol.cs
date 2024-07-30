using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class LabelSymbol(SyntaxNode Syntax, string Name, ModuleSymbol ContainingModule)
    : Symbol(BoundKind.LabelSymbol, Syntax, Name, Predefined.Never, ContainingModule, IsStatic: true, IsReadOnly: true)
{
    public LabelSymbol(string name, ModuleSymbol containingModule)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, containingModule)
    {
    }
}
