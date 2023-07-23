using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class WhileStatement(SyntaxTree SyntaxTree, Token While, Token OpenParenthesis, Expression Condition, Token CloseParenthesis, Statement Body)
    : Statement(SyntaxNodeKind.WhileStatement, SyntaxTree)
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
    public override string ToString() => base.ToString();
}