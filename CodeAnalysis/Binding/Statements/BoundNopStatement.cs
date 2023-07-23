using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundNopStatement(SyntaxNode Syntax)
    : BoundStatement(BoundNodeKind.NopStatement, Syntax)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Descendants() => Enumerable.Empty<INode>();
}
