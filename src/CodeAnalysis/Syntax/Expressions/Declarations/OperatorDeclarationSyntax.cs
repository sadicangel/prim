using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions.Declarations;
public sealed record class OperatorDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken OperatorToken,
    SyntaxToken ColonToken,
    LambdaTypeSyntax Type,
    SyntaxToken EqualsToken,
    ExpressionSyntax Body)
    : MemberDeclarationSyntax(SyntaxKind.OperatorDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return OperatorToken;
        yield return ColonToken;
        yield return Type;
        yield return EqualsToken;
        yield return Body;
    }
}
