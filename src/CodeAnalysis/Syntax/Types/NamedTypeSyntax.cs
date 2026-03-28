using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Types;

public sealed record class NamedTypeSyntax(NameSyntax Name)
    : TypeSyntax(SyntaxKind.NamedType)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
    }
}
