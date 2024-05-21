namespace CodeAnalysis.Syntax.Types;

public sealed record class ParameterSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken,
    SyntaxToken ColonToken,
    TypeSyntax Type)
    : SyntaxNode(SyntaxKind.Parameter, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
        yield return ColonToken;
        yield return Type;
    }
}