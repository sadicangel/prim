using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundReturnStatement(SyntaxNode Syntax, BoundExpression? Expression)
    : BoundStatement(BoundNodeKind.ReturnStatement, Syntax)
{
    public BoundReturnStatement(SyntaxNode syntax) : this(syntax, Expression: null) { }
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        if (Expression is not null)
            yield return Expression;
    }
}