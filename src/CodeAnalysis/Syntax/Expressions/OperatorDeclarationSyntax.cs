using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class OperatorDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken OperatorToken,
    SyntaxToken ColonToken,
    FunctionTypeSyntax Type,
    SyntaxToken ColonOrEqualsToken,
    ExpressionSyntax Body)
    : MemberDeclarationSyntax(SyntaxKind.OperatorDeclaration, SyntaxTree)
{
    public bool IsMutable { get => ColonOrEqualsToken.SyntaxKind is SyntaxKind.EqualsToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return OperatorToken;
        yield return ColonToken;
        yield return Type;
        yield return ColonOrEqualsToken;
        yield return Body;
    }
}
