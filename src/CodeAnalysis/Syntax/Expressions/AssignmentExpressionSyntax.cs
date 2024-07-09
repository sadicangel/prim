namespace CodeAnalysis.Syntax.Expressions;
public sealed record class AssignmentExpressionSyntax(
    SyntaxTree SyntaxTree,
    ExpressionSyntax Left,
    SyntaxToken OperatorToken,
    ExpressionSyntax Right)
    : ExpressionSyntax(SyntaxKind.AssignmentExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}
