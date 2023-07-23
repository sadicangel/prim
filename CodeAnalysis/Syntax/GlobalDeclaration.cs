using CodeAnalysis.Syntax.Statements;

namespace CodeAnalysis.Syntax;

public sealed record class GlobalDeclaration(SyntaxTree SyntaxTree, Declaration Declaration)
    : GlobalSyntaxNode(SyntaxNodeKind.GlobalDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Declaration;
    }
}
