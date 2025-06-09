namespace CodeAnalysis.Syntax.Expressions;
public sealed record class InitClauseSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken EqualsToken,
    ExpressionSyntax Expression)
    : SyntaxNode(SyntaxKind.InitClause, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return EqualsToken;
        yield return Expression;
    }
}
