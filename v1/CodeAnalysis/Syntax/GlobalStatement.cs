using CodeAnalysis.Syntax.Statements;

namespace CodeAnalysis.Syntax;

public sealed record class GlobalStatement(SyntaxTree SyntaxTree, Statement Statement)
    : GlobalSyntaxNode(SyntaxNodeKind.GlobalStatement, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Statement;
    }
}