namespace CodeAnalysis.Syntax.Types;

public sealed record class PointerTypeSyntax(
    SyntaxTree SyntaxTree,
    TypeSyntax ElementType,
    SyntaxToken StarToken)
    : TypeSyntax(SyntaxKind.PointerType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElementType;
        yield return StarToken;
    }
}
