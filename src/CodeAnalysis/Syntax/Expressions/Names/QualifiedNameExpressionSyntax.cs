namespace CodeAnalysis.Syntax.Expressions.Names;

public record class QualifiedNameExpressionSyntax(
    SyntaxTree SyntaxTree,
    NameExpressionSyntax Left,
    SyntaxToken ColonColonToken,
    SimpleNameExpressionSyntax Right)
    : NameExpressionSyntax(SyntaxKind.QualifiedNameExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return ColonColonToken;
        yield return Right;
    }
}
