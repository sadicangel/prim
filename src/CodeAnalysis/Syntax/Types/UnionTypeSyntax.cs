namespace CodeAnalysis.Syntax.Types;

public sealed record class UnionTypeSyntax(SeparatedSyntaxList<TypeSyntax> Types)
    : TypeSyntax(SyntaxKind.UnionType)
{
    public override IEnumerable<SyntaxNode> Children() => Types.SyntaxNodes;
}
