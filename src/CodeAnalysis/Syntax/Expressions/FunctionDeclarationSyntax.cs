using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class FunctionDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken,
    SyntaxToken ColonToken,
    FunctionTypeSyntax Type,
    SyntaxToken OperatorToken,
    ExpressionSyntax Body)
    : DeclarationSyntax(SyntaxKind.FunctionDeclaration, SyntaxTree)
{
    public bool IsReadOnly { get => OperatorToken.SyntaxKind is SyntaxKind.ColonToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
        yield return ColonToken;
        yield return Type;
        yield return OperatorToken;
        yield return Body;
    }
}
