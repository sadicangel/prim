using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class PropertyDeclarationSyntax(
    SyntaxTree SyntaxTree,
    IdentifierNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    TypeSyntax Type,
    InitValueExpressionSyntax? InitValue,
    SyntaxToken? SemicolonToken)
    : MemberDeclarationSyntax(SyntaxKind.PropertyDeclaration, SyntaxTree)
{
    public bool IsReadOnly { get => InitValue?.IsReadOnly ?? false; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        yield return Type;
        if (InitValue is not null)
            yield return InitValue;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
