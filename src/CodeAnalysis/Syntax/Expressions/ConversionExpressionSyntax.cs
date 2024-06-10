using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class ConversionExpressionSyntax(
    SyntaxTree SyntaxTree,
    ExpressionSyntax Expression,
    SyntaxToken AsKeyword,
    TypeSyntax Type)
    : ExpressionSyntax(SyntaxKind.ConversionExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return AsKeyword;
        yield return Type;
    }
}
