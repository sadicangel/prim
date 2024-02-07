using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundDeclarationExpression(
    SyntaxNode SyntaxNode,
    Symbol Symbol,
    BoundExpression Expression
)
    : BoundExpression(BoundNodeKind.DeclarationExpression, SyntaxNode, Symbol.Type)
{
    public DeclarationKind DeclarationKind { get => ((DeclarationExpression)SyntaxNode).DeclarationKind; }

    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
    }
}
