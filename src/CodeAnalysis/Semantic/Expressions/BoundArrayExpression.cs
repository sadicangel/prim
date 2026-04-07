using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundArrayExpression(
    SyntaxNode Syntax,
    TypeSymbol Type,
    ImmutableArray<BoundExpression> Elements)
    : BoundExpression(BoundKind.ArrayExpression, Syntax, Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => Elements;
}
