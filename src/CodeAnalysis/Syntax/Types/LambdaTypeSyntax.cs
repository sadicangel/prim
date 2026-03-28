namespace CodeAnalysis.Syntax.Types;

public sealed record class LambdaTypeSyntax(
    SyntaxToken ParenthesisOpenToken,
    SeparatedSyntaxList<TypeSyntax> Parameters,
    SyntaxToken ParenthesisCloseToken,
    SyntaxToken ArrowReturnToken,
    TypeSyntax ReturnType)
    : TypeSyntax(SyntaxKind.LambdaType)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpenToken;
        foreach (var parameter in Parameters.SyntaxNodes)
            yield return parameter;
        yield return ParenthesisCloseToken;
        yield return ArrowReturnToken;
        yield return ReturnType;
    }
}
