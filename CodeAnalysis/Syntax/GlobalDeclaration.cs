using CodeAnalysis.Syntax.Statements;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class GlobalDeclaration(SyntaxTree SyntaxTree, Declaration Declaration)
    : GlobalSyntaxNode(SyntaxNodeKind.GlobalDeclaration, SyntaxTree)
{
    public override TextSpan Span { get => Declaration.Span; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Declaration;
    }
}
