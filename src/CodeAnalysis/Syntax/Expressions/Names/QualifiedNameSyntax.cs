namespace CodeAnalysis.Syntax.Expressions.Names;

public record class QualifiedNameSyntax(
    SyntaxTree SyntaxTree,
    NameSyntax Left,
    SyntaxToken ColonColonToken,
    SimpleNameSyntax Right)
    : NameSyntax(SyntaxKind.QualifiedNameExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return ColonColonToken;
        yield return Right;
    }
}
