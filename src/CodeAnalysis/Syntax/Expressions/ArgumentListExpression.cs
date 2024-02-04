using System.Collections;

namespace CodeAnalysis.Syntax.Expressions;

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