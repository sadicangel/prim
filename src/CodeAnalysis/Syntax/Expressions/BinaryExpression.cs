namespace CodeAnalysis.Syntax.Expressions;
public record class BinaryExpression(
    SyntaxTree SyntaxTree,
    Expression Left,
    Token Operator,
    Expression Right
)
    : Expression(SyntaxNodeKind.BinaryExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }
}