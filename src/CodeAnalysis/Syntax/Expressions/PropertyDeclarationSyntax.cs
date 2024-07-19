
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class PropertyDeclarationSyntax(
    SyntaxTree SyntaxTree,
    IdentifierNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    TypeSyntax Type,
    SyntaxToken ColonOrEqualsToken,
    ExpressionSyntax Init)
    : MemberDeclarationSyntax(SyntaxKind.PropertyDeclaration, SyntaxTree)
{
    public bool IsReadOnly { get => ColonOrEqualsToken.SyntaxKind is SyntaxKind.ColonToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        yield return Type;
        yield return ColonOrEqualsToken;
        yield return Init;
    }
}
