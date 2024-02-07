using System.Text;

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

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder.Token(BraceOpen);
        foreach (var expression in Expressions)
            builder.Node(expression);
        builder.Token(BraceClose);
    }
}
