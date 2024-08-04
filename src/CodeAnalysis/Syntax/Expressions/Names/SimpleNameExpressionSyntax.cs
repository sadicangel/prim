namespace CodeAnalysis.Syntax.Expressions.Names;
public record class SimpleNameExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken)
    : NameExpressionSyntax(SyntaxKind.IdentifierNameExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
    }
}
