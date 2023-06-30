namespace CodeAnalysis.Syntax;

public sealed record class ForStatement(Token For, Token OpenParenthesis, Token Let, Token Identifier, Token In, Expression LowerBound, Token RangeToken, Expression UpperBound, Token CloseParenthesis, Statement Body) : Statement(SyntaxNodeKind.ForStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return For;
        yield return OpenParenthesis;
        yield return Let;
        yield return Identifier;
        yield return In;
        yield return LowerBound;
        yield return RangeToken;
        yield return UpperBound;
        yield return CloseParenthesis;
        yield return Body;
    }
}
