namespace CodeAnalysis.Syntax;

public sealed record class CallExpression(Token IdentifierToken, Token OpenParenthesis, SeparatedNodeList<Expression> Arguments, Token CloseParenthesis) : Expression(SyntaxNodeKind.CallExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return IdentifierToken;
        yield return OpenParenthesis;
        foreach (var node in Arguments.Nodes)
            yield return node;
        yield return CloseParenthesis;
    }
}
