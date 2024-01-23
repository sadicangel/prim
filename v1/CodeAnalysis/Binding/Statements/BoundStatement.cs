using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal abstract record class BoundStatement(BoundNodeKind NodeKind, SyntaxNode Syntax)
    : BoundNode(NodeKind, Syntax)
{
    public abstract T Accept<T>(IBoundStatementVisitor<T> visitor);
    public override string ToString() => base.ToString();
}
