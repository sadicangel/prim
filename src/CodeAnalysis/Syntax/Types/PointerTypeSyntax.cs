namespace CodeAnalysis.Syntax.Types;

public sealed record class PointerTypeSyntax(TypeSyntax ElementType, SyntaxToken StarToken)
    : TypeSyntax(SyntaxKind.PointerType)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElementType;
        yield return StarToken;
    }
}
