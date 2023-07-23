using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundConditionalGotoStatement(SyntaxNode Syntax, LabelSymbol Label, BoundExpression Condition, bool JumpIfTrue = true)
    : BoundStatement(BoundNodeKind.ConditionalGotoStatement, Syntax)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Descendants()
    {
        yield return Label;
    }
}
