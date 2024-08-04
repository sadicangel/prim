
using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class PropertyInitExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken DotToken,
    SimpleNameExpressionSyntax Name,
    SyntaxToken EqualsToken,
    ExpressionSyntax Init)
    : ExpressionSyntax(SyntaxKind.PropertyInitExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return DotToken;
        yield return Name;
        yield return EqualsToken;
        yield return Init;
    }
}
