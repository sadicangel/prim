using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class FunctionDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken,
    SyntaxToken ColonToken,
    FunctionTypeSyntax Type,
    SyntaxToken OperatorToken,
    ExpressionSyntax Expression)
    : DeclarationSyntax(SyntaxKind.FunctionDeclaration, SyntaxTree)
{
    public bool IsMutable { get => OperatorToken.SyntaxKind is SyntaxKind.EqualsToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
        yield return ColonToken;
        yield return Type;
        yield return OperatorToken;
        yield return Expression;
    }
}