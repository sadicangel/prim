namespace CodeAnalysis.Syntax.Expressions;

public sealed record class BlockExpression(
    SyntaxTree SyntaxTree,
    Token BraceOpen,
    IReadOnlyList<Expression> Expressions,
    Token BraceClose
)
    : Expression(SyntaxNodeKind.BlockExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BraceOpen;
        foreach (var expression in Expressions)
            yield return expression;
        yield return BraceClose;
    }
}
