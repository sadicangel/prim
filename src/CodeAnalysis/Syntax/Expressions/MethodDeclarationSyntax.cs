using CodeAnalysis.Syntax.Expressions.Names;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class MethodDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    LambdaTypeSyntax Type,
    SyntaxToken EqualsToken,
    ExpressionSyntax Body)
    : MemberDeclarationSyntax(SyntaxKind.MethodDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        yield return Type;
        yield return EqualsToken;
        yield return Body;
    }
}
