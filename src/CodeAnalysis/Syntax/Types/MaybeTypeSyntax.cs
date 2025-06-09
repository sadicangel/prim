namespace CodeAnalysis.Syntax.Types;

public sealed record class MaybeTypeSyntax(
    SyntaxTree SyntaxTree,
    TypeSyntax UnderlyingType,
    SyntaxToken HookToken)
    : TypeSyntax(SyntaxKind.MaybeType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return UnderlyingType;
        yield return HookToken;
    }
}
