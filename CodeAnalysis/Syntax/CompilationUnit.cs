using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class CompilationUnit(Statement Statement, Token Eof) : Node(NodeKind.CompilationUnit)
{
    public override TextSpan Span { get => Statement.Span; }

    public override IEnumerable<Node> GetChildren() => Statement.GetChildren();
}