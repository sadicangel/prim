using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Types;

public sealed record class ArrayTypeSyntax(
    SyntaxTree SyntaxTree,
    TypeSyntax ElementType,
    SyntaxToken BracketOpenToken,
    ExpressionSyntax Length,
    SyntaxToken BracketCloseToken)
    : TypeSyntax(SyntaxKind.ArrayType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElementType;
        yield return BracketOpenToken;
        yield return Length;
        yield return BracketCloseToken;
    }
}
