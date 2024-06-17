using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionSymbol(SyntaxNode Syntax, string Name, FunctionType Type)
    : Symbol(BoundKind.Function, Syntax, Name)
{
    public override FunctionType Type { get; } = Type;
}
