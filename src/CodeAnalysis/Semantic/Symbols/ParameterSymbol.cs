using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class ParameterSymbol(SyntaxNode Syntax, string Name, TypeSymbol Type, Symbol ContainingSymbol)
    : Symbol(BoundKind.ParameterSymbol, Syntax, Name, Type, ContainingSymbol, ContainingModule: ContainingSymbol.ContainingModule, Modifiers.None)
{
    public override IEnumerable<Symbol> Children() => [];
}
