namespace CodeAnalysis.Syntax.Expressions;

public sealed record class AssignmentExpressionSyntax(
    ExpressionSyntax Left,
    SyntaxToken EqualsToken,
    ExpressionSyntax Right)
    : ExpressionSyntax(SyntaxKind.AssignmentExpression)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return EqualsToken;
        yield return Right;
    }
}
