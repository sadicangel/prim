using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class ParameterSymbol(SyntaxNode SyntaxNode, Parameter Parameter)
    : Symbol(BoundKind.Parameter, SyntaxNode, Parameter.Name)
{
    public override PrimType Type { get; } = Parameter.Type;
}
