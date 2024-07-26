namespace CodeAnalysis.Syntax.Types;

public sealed record class PointerTypeSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken HookToken,
    SyntaxToken? ParenthesisOpenToken,
    TypeSyntax ElementType,
    SyntaxToken? ParenthesisCloseToken)
    : TypeSyntax(SyntaxKind.PointerType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return HookToken;
        if (ParenthesisOpenToken is not null)
            yield return ParenthesisOpenToken;
        yield return ElementType;
        if (ParenthesisCloseToken is not null)
            yield return ParenthesisCloseToken;
    }
}
