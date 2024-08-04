using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class StructDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    SyntaxToken StructKeyword,
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
        yield return StructKeyword;
        yield return ColonOrEquals;
        yield return BraceOpenToken;
        foreach (var member in Members)
            yield return member;
        yield return BraceCloseToken;
    }
}
