
namespace CodeAnalysis.Syntax.Types;
public sealed record class UnionTypeSyntax(SyntaxTree SyntaxTree, SeparatedSyntaxList<TypeSyntax> Types)
    : TypeSyntax(SyntaxKind.UnionType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        foreach (var node in Types.SyntaxNodes)
            yield return node;
    }
}
