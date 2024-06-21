namespace CodeAnalysis.Syntax.Expressions;
public sealed record class IndexExpressionSyntax(
    SyntaxTree SyntaxTree,
    ExpressionSyntax Expression,
    SyntaxToken BracketOpen,
    ExpressionSyntax Index,
    SyntaxToken BracketClose)
    : ExpressionSyntax(SyntaxKind.IndexExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return BracketOpen;
        yield return Index;
        yield return BracketClose;
    }
}
