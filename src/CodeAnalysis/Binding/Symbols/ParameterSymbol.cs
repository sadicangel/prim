using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class ParameterSymbol(SyntaxNode SyntaxNode, string Name, PrimType Type)
    : Symbol(BoundKind.Parameter, SyntaxNode, Name)
{
    public override PrimType Type { get; } = Type;
}
