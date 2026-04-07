using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundStructExpression(
    SyntaxNode Syntax,
    StructTypeSymbol StructType,
    ImmutableArray<BoundPropertyExpression> Properties)
    : BoundExpression(BoundKind.StructExpression, Syntax, StructType)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => Properties;
}
