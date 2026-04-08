using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundBlockExpression(
    SyntaxNode Syntax,
    TypeSymbol Type,
    ImmutableArray<BoundExpression> Expressions)
    : BoundExpression(BoundKind.BlockExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children() => Expressions;
}
