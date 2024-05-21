namespace CodeAnalysis.Syntax.Types;
public sealed record class NamedTypeSyntax(SyntaxTree SyntaxTree, SyntaxToken IdentifierToken)
    : TypeSyntax(SyntaxKind.NamedType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
    }
}
