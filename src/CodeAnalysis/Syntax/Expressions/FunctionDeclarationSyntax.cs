using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class FunctionDeclarationSyntax(
    SyntaxTree SyntaxTree,
    IdentifierNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    LambdaTypeSyntax Type,
    SyntaxToken ColonOrEqualsToken,
    ExpressionSyntax Body)
    : DeclarationSyntax(SyntaxKind.FunctionDeclaration, SyntaxTree)
{
    public bool IsReadOnly { get => ColonOrEqualsToken.SyntaxKind is SyntaxKind.ColonToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        yield return Type;
        yield return ColonOrEqualsToken;
        yield return Body;
    }
}
