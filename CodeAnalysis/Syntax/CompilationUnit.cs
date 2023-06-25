using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class CompilationUnit(Statement Statement, Token Eof) : SyntaxNode(SyntaxNodeKind.CompilationUnit)
{
    public override TextSpan Span { get => Statement.Span; }

    public override IEnumerable<SyntaxNode> GetChildren() => Statement.GetChildren();
}