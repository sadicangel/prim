using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class ReturnStatement(SyntaxTree SyntaxTree, Token Return, Expression? Expression, Token Semicolon)
    : Statement(SyntaxNodeKind.ReturnStatement, SyntaxTree)
{
    public ReturnStatement(SyntaxTree syntaxTree, Token @return, Token semicolon) : this(syntaxTree, @return, Expression: null, semicolon) { }

    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Return;
        if (Expression is not null)
            yield return Expression;
        yield return Semicolon;
    }
    public override string ToString() => base.ToString();
}