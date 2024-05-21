namespace CodeAnalysis.Syntax.Types;

public sealed record class PredefinedTypeSyntax(SyntaxTree SyntaxTree, SyntaxToken IdentifierToken)
    : TypeSyntax(SyntaxKind.PredefinedType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
    }
}
