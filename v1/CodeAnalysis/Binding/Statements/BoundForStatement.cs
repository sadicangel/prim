using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundForStatement(SyntaxNode Syntax, VariableSymbol Variable, BoundExpression LowerBound, BoundExpression UpperBound, BoundStatement Body, LabelSymbol Break, LabelSymbol Continue)
    : BoundLoopBodyStatement(BoundNodeKind.ForStatement, Syntax, Break, Continue)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Children()
    {
        yield return Continue;
        yield return Variable;
        yield return LowerBound;
        yield return UpperBound;
        yield return Body;
        yield return Break;
    }
}
