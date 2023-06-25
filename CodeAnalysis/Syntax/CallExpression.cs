namespace CodeAnalysis.Syntax;

public sealed record class CallExpression(Token IdentifierToken, Token OpenParenthesis, SeparatedNodeList<Expression> Arguments, Token CloseParenthesis) : Expression(NodeKind.CallExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<Node> GetChildren()
    {
        yield return IdentifierToken;
        yield return OpenParenthesis;
        foreach (var node in Arguments.Nodes)
            yield return node;
        yield return CloseParenthesis;
    }
}
