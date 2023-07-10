using CodeAnalysis.Syntax.Statements;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class GlobalStatement(SyntaxTree SyntaxTree, Statement Statement)
    : GlobalSyntaxNode(SyntaxNodeKind.GlobalStatement, SyntaxTree)
{
    public override TextSpan Span { get => Statement.Span; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Statement;
    }
}