namespace CodeAnalysis.Syntax.Types;

public sealed record class OptionTypeSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken HookToken,
    TypeSyntax UnderlyingType)
    : TypeSyntax(SyntaxKind.OptionType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return HookToken;
        yield return UnderlyingType;
    }
}