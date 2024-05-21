namespace CodeAnalysis.Syntax.Types;
public sealed record class FunctionTypeSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ParenthesisOpenToken,
    ParameterSyntaxList Parameters,
    SyntaxToken ParenthesisCloseToken,
    SyntaxToken ArrowToken,
    TypeSyntax ReturnType)
    : TypeSyntax(SyntaxKind.FunctionType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpenToken;
        yield return Parameters;
        yield return ParenthesisCloseToken;
        yield return ArrowToken;
        yield return ReturnType;
    }
}