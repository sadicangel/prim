namespace CodeAnalysis.Syntax.Expressions;
public sealed record class UnaryExpressionSyntax(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    SyntaxToken Operator,
    ExpressionSyntax Operand)
    : ExpressionSyntax(SyntaxKind, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Operator;
        yield return Operand;
    }
}
