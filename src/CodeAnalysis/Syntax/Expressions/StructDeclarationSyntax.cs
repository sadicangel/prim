using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class StructDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    SyntaxToken StructKeyword,
    SyntaxToken EqualsToken,
    SyntaxToken BraceOpenToken,
    SyntaxList<MemberDeclarationSyntax> Members,
    SyntaxToken BraceCloseToken)
    : DeclarationSyntax(SyntaxKind.StructDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        yield return StructKeyword;
        yield return EqualsToken;
        yield return BraceOpenToken;
        foreach (var member in Members)
            yield return member;
        yield return BraceCloseToken;
    }
}
