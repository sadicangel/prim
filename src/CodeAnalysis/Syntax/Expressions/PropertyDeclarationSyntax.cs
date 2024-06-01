
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class PropertyDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken,
    SyntaxToken ColonToken,
    TypeSyntax Type,
    SyntaxToken OperatorToken,
    ExpressionSyntax Init)
    : DeclarationSyntax(SyntaxKind.PropertyDeclaration, SyntaxTree)
{
    public bool IsReadOnly { get => OperatorToken.SyntaxKind is SyntaxKind.ColonToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
        yield return ColonToken;
        yield return Type;
        yield return OperatorToken;
        yield return Init;
    }
}
