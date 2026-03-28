using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Types;

public sealed record class ArrayTypeSyntax(
    TypeSyntax ElementType,
    SyntaxToken BracketOpenToken,
    ExpressionSyntax? Length,
    SyntaxToken BracketCloseToken)
    : TypeSyntax(SyntaxKind.ArrayType)
{
    public ArrayTypeSyntax(TypeSyntax elementType, SyntaxToken bracketOpenToken, SyntaxToken bracketCloseToken)
        : this(elementType, bracketOpenToken, null, bracketCloseToken) { }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElementType;
        yield return BracketOpenToken;
        if (Length is not null) yield return Length;
        yield return BracketCloseToken;
    }
}
