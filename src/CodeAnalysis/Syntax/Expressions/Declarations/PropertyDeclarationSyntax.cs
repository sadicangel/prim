using CodeAnalysis.Syntax.Expressions.Names;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions.Declarations;
public sealed record class PropertyDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    TypeSyntax Type,
    InitValueExpressionSyntax? InitValue,
    SyntaxToken? SemicolonToken)
    : MemberDeclarationSyntax(SyntaxKind.PropertyDeclaration, SyntaxTree)
{
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
