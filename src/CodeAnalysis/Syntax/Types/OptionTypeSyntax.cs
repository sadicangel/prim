namespace CodeAnalysis.Syntax.Types;

public sealed record class OptionTypeSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken HookToken,
    SyntaxToken? ParenthesisOpenToken,
    TypeSyntax UnderlyingType,
    SyntaxToken? ParenthesisCloseToken)
    : TypeSyntax(SyntaxKind.OptionType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return HookToken;
        if (ParenthesisOpenToken is not null)
            yield return ParenthesisOpenToken;
        yield return UnderlyingType;
        if (ParenthesisCloseToken is not null)
            yield return ParenthesisCloseToken;
    }
}
