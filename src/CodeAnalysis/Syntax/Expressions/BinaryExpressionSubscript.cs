namespace CodeAnalysis.Syntax.Expressions;

public record class BinaryExpressionSubscript(
    SyntaxTree SyntaxTree,
    Expression Left,
    Token BracketOpen,
    Expression Right,
    Token BracketClose
)
    : BinaryExpression(SyntaxTree, Left, BracketOpen, Right)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return BracketOpen;
        yield return Right;
        yield return BracketClose;
    }
}