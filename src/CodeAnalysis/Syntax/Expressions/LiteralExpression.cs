namespace CodeAnalysis.Syntax.Expressions;
public sealed record class LiteralExpression(
    SyntaxTree SyntaxTree,
    Token Literal,
    PrimType Type,
    object? Value)
    : Expression(SyntaxNodeKind.LiteralExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children() { yield return Literal; }
}
