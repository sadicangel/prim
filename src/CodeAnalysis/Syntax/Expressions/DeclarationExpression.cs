namespace CodeAnalysis.Syntax.Expressions;

public sealed record class DeclarationExpression(
    SyntaxTree SyntaxTree,
    Token Identifier,
    Token Colon,
    Token? Mutable,
    TypeSyntax Type,
    Token Equal,
    Expression Expression
)
    : IdentifierExpression(SyntaxNodeKind.DeclarationExpression, SyntaxTree, Identifier, Mutable is not null)
{

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
        yield return Colon;
        if (Mutable is not null)
            yield return Mutable;
        yield return Type;
        yield return Equal;
        yield return Expression;
    }
}