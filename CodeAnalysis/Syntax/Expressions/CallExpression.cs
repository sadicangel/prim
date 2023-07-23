namespace CodeAnalysis.Syntax.Expressions;

public sealed record class CallExpression(SyntaxTree SyntaxTree, Token Identifier, Token OpenParenthesis, SeparatedNodeList<Expression> Arguments, Token CloseParenthesis)
    : Expression(SyntaxNodeKind.CallExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
        yield return OpenParenthesis;
        foreach (var node in Arguments.Nodes)
            yield return node;
        yield return CloseParenthesis;
    }

    public override string ToString() => base.ToString();
}
