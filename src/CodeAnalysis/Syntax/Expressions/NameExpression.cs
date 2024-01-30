namespace CodeAnalysis.Syntax.Expressions;
public sealed record class NameExpression(
    SyntaxTree SyntaxTree,
    Token Identifier
)
    : IdentifierExpression(SyntaxNodeKind.NameExpression, SyntaxTree, Identifier)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
    }
}
