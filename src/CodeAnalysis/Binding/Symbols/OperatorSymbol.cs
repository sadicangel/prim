using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class OperatorSymbol(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    Operator Operator)
    : Symbol(BoundKind, SyntaxNode, $"{Operator.Name}<{Operator.Type.Name}>")
{
    public override PrimType Type { get; } = Operator.Type;
}
