namespace CodeAnalysis.Syntax.Expressions.ControlFlow;

public sealed record class ElseClauseExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ElseKeyword,
    ExpressionSyntax Else)
    : ExpressionSyntax(SyntaxKind.ElseClauseExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElseKeyword;
        yield return Else;
    }
}
