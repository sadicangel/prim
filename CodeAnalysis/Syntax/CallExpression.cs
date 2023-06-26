namespace CodeAnalysis.Syntax;

public sealed record class CallExpression(Token Identifier, Token OpenParenthesis, SeparatedNodeList<Expression> Arguments, Token CloseParenthesis) : Expression(SyntaxNodeKind.CallExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return OpenParenthesis;
        foreach (var node in Arguments.Nodes)
            yield return node;
        yield return CloseParenthesis;
    }
}
