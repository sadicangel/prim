
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class EmptyExpressionSyntax(SyntaxTree SyntaxTree, SyntaxToken SemicolonToken)
    : ExpressionSyntax(SyntaxKind.EmptyExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return SemicolonToken;
    }
}
