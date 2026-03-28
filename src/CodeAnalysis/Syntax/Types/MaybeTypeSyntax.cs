namespace CodeAnalysis.Syntax.Types;

public sealed record class MaybeTypeSyntax(
    TypeSyntax UnderlyingType,
    SyntaxToken HookToken)
    : TypeSyntax(SyntaxKind.MaybeType)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return UnderlyingType;
        yield return HookToken;
    }
}
