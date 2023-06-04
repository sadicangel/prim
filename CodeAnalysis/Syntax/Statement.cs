namespace CodeAnalysis.Syntax;

public abstract record class Statement(NodeKind Kind) : Node(Kind)
{
    public abstract T Accept<T>(IStatementVisitor<T> visitor);
}