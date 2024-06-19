using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionSymbol(SyntaxNode Syntax, string Name, FunctionType Type)
    : Symbol(BoundKind.FunctionSymbol, Syntax, Name)
{
    public override FunctionType Type { get; } = Type;
}
