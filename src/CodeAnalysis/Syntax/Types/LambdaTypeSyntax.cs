namespace CodeAnalysis.Syntax.Types;
public sealed record class LambdaTypeSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ParenthesisOpenToken,
    SeparatedSyntaxList<ParameterSyntax> Parameters,
    SyntaxToken ParenthesisCloseToken,
    SyntaxToken ArrowToken,
    TypeSyntax ReturnType)
    : TypeSyntax(SyntaxKind.LambdaType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpenToken;
        foreach (var parameter in Parameters.SyntaxNodes)
            yield return parameter;
        yield return ParenthesisCloseToken;
        yield return ArrowToken;
        yield return ReturnType;
    }
}
