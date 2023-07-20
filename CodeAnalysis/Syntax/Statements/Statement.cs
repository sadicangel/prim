namespace CodeAnalysis.Syntax.Statements;

public abstract record class Statement(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : SyntaxNode(NodeKind, SyntaxTree)
{
    public abstract T Accept<T>(ISyntaxStatementVisitor<T> visitor);
}