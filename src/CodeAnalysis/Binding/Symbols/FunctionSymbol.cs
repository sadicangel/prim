using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionSymbol(SyntaxNode SyntaxNode, string Name, FunctionType Type)
    : Symbol(BoundKind.Function, SyntaxNode, Name)
{
    public override FunctionType Type { get; } = Type;
}
