namespace CodeAnalysis.Syntax;

public sealed record class ParameterSyntax(
    SyntaxTree SyntaxTree,
    Token Identifier,
    Token Colon,
    TypeSyntax TypeNode
)
    : SyntaxNode(SyntaxNodeKind.Parameter, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
        yield return Colon;
        yield return TypeNode;
    }
}