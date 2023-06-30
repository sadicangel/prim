namespace CodeAnalysis.Syntax;

public sealed record class WhileStatement(Token While, Token OpenParenthesis, Expression Condition, Token CloseParenthesis, Statement Body) : Statement(SyntaxNodeKind.WhileStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return While;
        yield return OpenParenthesis;
        yield return Condition;
        yield return CloseParenthesis;
        yield return Body;
    }
}