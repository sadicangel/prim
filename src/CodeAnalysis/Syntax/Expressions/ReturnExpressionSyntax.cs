
namespace CodeAnalysis.Syntax.Expressions;

public sealed record class ReturnExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ReturnKeyword,
    ExpressionSyntax? Expression,
    SyntaxToken? SemicolonToken)
    : ExpressionSyntax(SyntaxKind.ReturnExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ReturnKeyword;
        if (Expression is not null)
            yield return Expression;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
