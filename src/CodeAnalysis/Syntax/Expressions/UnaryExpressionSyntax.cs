namespace CodeAnalysis.Syntax.Expressions;
public sealed record class UnaryExpressionSyntax(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    SyntaxToken OperatorToken,
    ExpressionSyntax Operand)
    : ExpressionSyntax(SyntaxKind, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}
