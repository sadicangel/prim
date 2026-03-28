using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class TypeClauseSyntax(SyntaxToken ColonToken, TypeSyntax Type)
    : SyntaxNode(SyntaxKind.TypeClause)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ColonToken;
        yield return Type;
    }
}
