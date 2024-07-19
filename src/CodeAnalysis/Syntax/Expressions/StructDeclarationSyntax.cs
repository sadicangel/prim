namespace CodeAnalysis.Syntax.Expressions;
public sealed record class StructDeclarationSyntax(
    SyntaxTree SyntaxTree,
    IdentifierNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    SyntaxToken StructToken,
    SyntaxToken ColonOrEquals,
    SyntaxToken BraceOpenToken,
    SyntaxList<MemberDeclarationSyntax> Members,
    SyntaxToken BraceCloseToken)
    : DeclarationSyntax(SyntaxKind.StructDeclaration, SyntaxTree)
{
    public bool IsReadOnly
    { get => ColonOrEquals.SyntaxKind is SyntaxKind.ColonToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        yield return StructToken;
        yield return ColonOrEquals;
        yield return BraceOpenToken;
        foreach (var member in Members)
            yield return member;
        yield return BraceCloseToken;
    }
}
