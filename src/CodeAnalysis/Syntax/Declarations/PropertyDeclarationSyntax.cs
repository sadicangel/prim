using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Names;

/*
 struct Point {
    x: i32 = 2;
    y: i32;
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
