namespace CodeAnalysis.Syntax.Expressions;
public sealed record class BinaryExpressionSyntax(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    ExpressionSyntax Left,
    SyntaxToken OperatorToken,
    ExpressionSyntax Right)
    : ExpressionSyntax(SyntaxKind, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}
