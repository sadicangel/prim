namespace CodeAnalysis.Syntax.Expressions;

public sealed record class InitClauseSyntax(SyntaxToken EqualsToken, ExpressionSyntax Expression)
    : SyntaxNode(SyntaxKind.InitClause)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return EqualsToken;
        yield return Expression;
    }
}
