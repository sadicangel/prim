using CodeAnalysis.Syntax.Expressions.Names;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions.Declarations;
public sealed record class VariableDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameSyntax Name,
    SyntaxToken ColonToken,
    TypeSyntax? Type,
    InitValueExpressionSyntax? InitValue,
    SyntaxToken? SemicolonToken)
    : DeclarationSyntax(SyntaxKind.VariableDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        if (Type is not null)
            yield return Type;
        if (InitValue is not null)
            yield return InitValue;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
