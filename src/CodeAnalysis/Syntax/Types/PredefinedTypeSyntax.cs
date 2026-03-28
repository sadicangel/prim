namespace CodeAnalysis.Syntax.Types;

public sealed record class PredefinedTypeSyntax(SyntaxToken PredefinedTypeToken)
    : TypeSyntax(SyntaxKind.PredefinedType)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return PredefinedTypeToken;
    }
}
