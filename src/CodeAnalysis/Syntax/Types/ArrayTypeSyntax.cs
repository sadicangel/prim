using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Types;

public sealed record class ArrayTypeSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken BracketOpenToken,
    TypeSyntax ElementType,
    SyntaxToken ColonToken,
    ExpressionSyntax Length,
    SyntaxToken BracketCloseToken)
    : TypeSyntax(SyntaxKind.ArrayType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BracketOpenToken;
        yield return ElementType;
        yield return ColonToken;
        yield return Length;
        yield return BracketCloseToken;
    }
}
