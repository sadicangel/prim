using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.ControlFlow;

internal sealed record class BoundIfElseExpression(
    SyntaxNode Syntax,
    BoundExpression Condition,
    BoundExpression Then,
    BoundExpression? Else,
    TypeSymbol Type)
    : BoundExpression(BoundKind.IfElseExpression, Syntax, Type)
{
    public BoundIfElseExpression(SyntaxNode Syntax, BoundExpression Condition, BoundExpression Then)
        : this(Syntax, Condition, Then, null, new UnionTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.UnionType), [Then.Type, Then.Type.ContainingModule.Unit], Then.Type.ContainingModule)) { }

    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        yield return Condition;
        yield return Then;
        if (Else is not null) yield return Else;
    }
}
