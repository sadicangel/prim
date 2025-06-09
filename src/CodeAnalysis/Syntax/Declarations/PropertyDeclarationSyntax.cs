using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Declarations;
public sealed record class PropertyDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameSyntax Name,
    TypeClauseSyntax TypeClause,
    InitClauseSyntax? InitClause,
    SyntaxToken SemicolonToken)
    : ExpressionSyntax(SyntaxKind.PropertyDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        if (TypeClause is not null)
            yield return TypeClause;
        if (InitClause is not null)
            yield return InitClause;
        yield return SemicolonToken;
    }
}

