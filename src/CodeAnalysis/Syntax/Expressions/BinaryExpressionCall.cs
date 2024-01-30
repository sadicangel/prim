using System.Collections;

namespace CodeAnalysis.Syntax.Expressions;

public record class BinaryExpressionCall(
    SyntaxTree SyntaxTree,
    Expression Left,
    Token ParenthesisOpen,
    ArgumentListExpression ArgumentList,
    Token ParenthesisClose
)
    : BinaryExpression(SyntaxTree, Left, ParenthesisOpen, ArgumentList)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return ParenthesisOpen;
        yield return Right;
        yield return ParenthesisClose;
    }
}

public sealed record class ArgumentListExpression(
    SyntaxTree SyntaxTree,
    SeparatedNodeList<Expression> Arguments
)
    : Expression(SyntaxNodeKind.ArgumentList, SyntaxTree), IReadOnlyList<Expression>
{
    public Expression this[int index] { get => Arguments[index]; }

    public int Count { get => Arguments.Count; }

    public override IEnumerable<SyntaxNode> Children() => Arguments;
    public IEnumerator<Expression> GetEnumerator() => Arguments.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
