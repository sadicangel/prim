using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Names;

/*
 Point = {
    x = 2,
    y = 3,
}
 */

namespace CodeAnalysis.Syntax.Declarations;

public sealed record class PropertyDeclarationSyntax(
    SimpleNameSyntax Name,
    TypeClauseSyntax TypeClause,
    InitClauseSyntax? InitClause,
    SyntaxToken SemicolonToken)
    : ExpressionSyntax(SyntaxKind.PropertyDeclaration)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return TypeClause;
        if (InitClause is not null)
            yield return InitClause;
        yield return SemicolonToken;
    }
}
