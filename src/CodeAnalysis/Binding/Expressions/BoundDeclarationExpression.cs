using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundDeclarationExpression(
    SyntaxNode SyntaxNode,
    Symbol Symbol,
    BoundExpression Expression
)
    : BoundExpression(BoundNodeKind.DeclarationExpression, SyntaxNode, Symbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
    }
}
