
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class StatementExpressionSyntax(
    SyntaxTree SyntaxTree,
    ExpressionSyntax Expression,
    SyntaxToken SemicolonToken)
    : ExpressionSyntax(SyntaxKind.StatementExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return SemicolonToken;
    }
}
