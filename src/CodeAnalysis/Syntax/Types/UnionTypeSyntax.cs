
namespace CodeAnalysis.Syntax.Types;
public sealed record class UnionTypeSyntax(SyntaxTree SyntaxTree, TypeSyntaxList Types)
    : TypeSyntax(SyntaxKind.UnionType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children() => Types;
}
