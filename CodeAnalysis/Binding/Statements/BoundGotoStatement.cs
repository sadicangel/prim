using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;
internal sealed record class BoundGotoStatement(SyntaxNode Syntax, LabelSymbol Label)
    : BoundStatement(BoundNodeKind.GotoStatement, Syntax)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Children()
    {
        yield return Label;
    }
}
