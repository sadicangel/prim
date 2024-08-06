namespace CodeAnalysis.Syntax.Expressions.Names;
public record class SimpleNameSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken)
    : NameSyntax(SyntaxKind.SimpleName, SyntaxTree)
{
    public override NameValue NameValue { get; } = new NameValue(IdentifierToken.Text.ToString());

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
    }
}
