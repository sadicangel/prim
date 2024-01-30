
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class EmptyExpression(
    SyntaxTree SyntaxTree,
    Token Semicolon
)
    : Expression(SyntaxNodeKind.EmptyExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Semicolon;
    }
}
