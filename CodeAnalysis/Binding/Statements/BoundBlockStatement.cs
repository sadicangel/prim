using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundBlockStatement(SyntaxNode Syntax, IReadOnlyList<BoundStatement> Statements)
    : BoundStatement(BoundNodeKind.BlockStatement, Syntax)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren() => Statements;
    public override string ToString() => base.ToString();
}