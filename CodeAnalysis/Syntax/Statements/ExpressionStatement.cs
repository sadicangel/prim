using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class ExpressionStatement(SyntaxTree SyntaxTree, Expression Expression, Token? Semicolon = null)
    : Statement(SyntaxNodeKind.ExpressionStatement, SyntaxTree)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Expression;
        if (Semicolon is not null)
            yield return Semicolon;
    }
    public override string ToString() => base.ToString();
}
