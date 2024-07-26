using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class LabelSymbol(SyntaxNode Syntax, string Name)
    : Symbol(BoundKind.LabelSymbol, Syntax, Name, PredefinedSymbols.Unit, IsStatic: true, IsReadOnly: true)
{
    public LabelSymbol(string name)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name)
    {
    }
}
