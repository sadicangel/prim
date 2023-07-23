using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundExpressionStatement(SyntaxNode Syntax, BoundExpression Expression)
    : BoundStatement(BoundNodeKind.ExpressionStatement, Syntax)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Expression;
    }
}
