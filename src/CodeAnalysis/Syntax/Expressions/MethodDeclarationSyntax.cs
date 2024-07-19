using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class MethodDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken,
    SyntaxToken ColonToken,
    LambdaTypeSyntax Type,
    SyntaxToken ColonOrEqualsToken,
    ExpressionSyntax Body)
    : MemberDeclarationSyntax(SyntaxKind.MethodDeclaration, SyntaxTree)
{
    public bool IsReadOnly { get => ColonOrEqualsToken.SyntaxKind is SyntaxKind.ColonToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
        yield return ColonToken;
        yield return Type;
        yield return ColonOrEqualsToken;
        yield return Body;
    }
}
