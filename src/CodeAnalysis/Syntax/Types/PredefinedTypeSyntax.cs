namespace CodeAnalysis.Syntax.Types;

public sealed record class PredefinedTypeSyntax(SyntaxTree SyntaxTree, SyntaxToken TypeKeywordToken)
    : TypeSyntax(SyntaxKind.PredefinedType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return TypeKeywordToken;
    }
}
