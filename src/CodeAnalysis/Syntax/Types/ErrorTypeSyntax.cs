namespace CodeAnalysis.Syntax.Types;

public sealed record class ErrorTypeSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken BangToken,
    SyntaxToken? ParenthesisOpenToken,
    TypeSyntax ValueType,
    SyntaxToken? ParenthesisCloseToken)
    : TypeSyntax(SyntaxKind.ErrorType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BangToken;
        if (ParenthesisOpenToken is not null)
            yield return ParenthesisOpenToken;
        yield return ValueType;
        if (ParenthesisCloseToken is not null)
            yield return ParenthesisCloseToken;
    }
}
