namespace CodeAnalysis.Syntax;

public sealed record class CallExpression(Token Identifier, Token OpenParenthesis, SeparatedNodeList<Expression> Arguments, Token CloseParenthesis) : Expression(NodeKind.CallExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Identifier;
        yield return OpenParenthesis;
        foreach (var node in Arguments.Nodes)
            yield return node;
        yield return CloseParenthesis;
    }
}
