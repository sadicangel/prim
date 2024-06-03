using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundOperatorDeclaration(
    SyntaxNode SyntaxNode,
    OperatorSymbol OperatorSymbol,
    BoundExpression Expression)
    : BoundMemberDeclaration(BoundKind.OperatorDeclaration, SyntaxNode, OperatorSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return OperatorSymbol;
        yield return Expression;
    }
}
