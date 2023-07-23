using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundWhileStatement(SyntaxNode Syntax, BoundExpression Condition, BoundStatement Body, LabelSymbol Break, LabelSymbol Continue)
    : BoundLoopBodyStatement(BoundNodeKind.WhileStatement, Syntax, Break, Continue)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Descendants()
    {
        yield return Continue;
        yield return Condition;
        yield return Body;
        yield return Break;
    }
}
