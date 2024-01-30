
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class BreakExpression(
    SyntaxTree SyntaxTree,
    Token Break,
    Expression Result
)
    : Expression(SyntaxNodeKind.BreakExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Break;
        yield return Result;
    }
}
