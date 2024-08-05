namespace CodeAnalysis.Syntax.Expressions.Names;
public record class SimpleNameSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken)
    : NameSyntax(SyntaxKind.SimpleName, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
    }
}
