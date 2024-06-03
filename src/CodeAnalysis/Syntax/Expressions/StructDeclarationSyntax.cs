namespace CodeAnalysis.Syntax.Expressions;
public sealed record class StructDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken,
    SyntaxToken ColonToken,
    SyntaxToken StructToken,
    SyntaxToken OperatorToken,
    SyntaxToken BraceOpenToken,
    SyntaxList<MemberDeclarationSyntax> Members,
    SyntaxToken BraceCloseToken)
    : DeclarationSyntax(SyntaxKind.StructDeclaration, SyntaxTree)
{
    public bool IsMutable { get => OperatorToken.SyntaxKind is SyntaxKind.EqualsToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
        yield return ColonToken;
        yield return StructToken;
        yield return OperatorToken;
        yield return BraceOpenToken;
        foreach (var member in Members)
            yield return member;
        yield return BraceCloseToken;
    }
}
