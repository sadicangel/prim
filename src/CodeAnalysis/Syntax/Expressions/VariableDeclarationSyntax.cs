using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class VariableDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken,
    SyntaxToken ColonToken,
    TypeSyntax? Type,
    SyntaxToken ColonOrEqualsToken,
    ExpressionSyntax Expression)
    : DeclarationSyntax(SyntaxKind.VariableDeclaration, SyntaxTree)
{
    public bool IsReadOnly { get => ColonOrEqualsToken.SyntaxKind is SyntaxKind.ColonToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
        yield return ColonToken;
        if (Type is not null)
            yield return Type;
        yield return ColonOrEqualsToken;
        yield return Expression;
    }
}
