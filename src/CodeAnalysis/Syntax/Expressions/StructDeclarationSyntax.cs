namespace CodeAnalysis.Syntax.Expressions;
public sealed record class StructDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken,
    SyntaxToken ColonToken,
    SyntaxToken TypeToken,
    SyntaxToken OperatorToken,
    ExpressionSyntax Expression)
    : DeclarationSyntax(SyntaxKind.StructDeclaration, SyntaxTree)
{
    public bool IsMutable { get => OperatorToken.SyntaxKind is SyntaxKind.EqualsToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
        yield return ColonToken;
        yield return TypeToken;
        yield return OperatorToken;
        yield return Expression;
    }
}