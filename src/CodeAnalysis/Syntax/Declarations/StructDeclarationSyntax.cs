using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Declarations;

public sealed record class StructDeclarationSyntax(
    SyntaxToken StructKeyword,
    SimpleNameSyntax Name,
    SyntaxToken BraceOpenToken,
    SyntaxList<PropertyDeclarationSyntax> Properties,
    SyntaxToken BraceCloseToken)
    : DeclarationSyntax(SyntaxKind.StructDeclaration)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return StructKeyword;
        yield return Name;
        yield return BraceOpenToken;
        foreach (var property in Properties)
            yield return property;
        yield return BraceCloseToken;
    }
}
