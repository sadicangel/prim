using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class TypeClauseSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ColonToken,
    TypeSyntax Type)
    : SyntaxNode(SyntaxKind.TypeClause, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ColonToken;
        yield return Type;
    }
}
