using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundArrayInitExpression(
    SyntaxNode Syntax,
    TypeSymbol Type,
    ImmutableArray<BoundExpression> Elements)
    : BoundExpression(BoundKind.ArrayInitExpression, Syntax, Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => Elements;
}
