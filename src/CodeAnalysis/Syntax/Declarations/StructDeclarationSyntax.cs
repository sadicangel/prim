using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Declarations;

public sealed record class StructDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken StructKeyword,
    SimpleNameSyntax Name,
    SyntaxToken BraceOpenToken,
    SyntaxList<PropertyDeclarationSyntax> Properties,
    SyntaxToken BraceCloseToken)
    : DeclarationSyntax(SyntaxKind.StructDeclaration, SyntaxTree)
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
