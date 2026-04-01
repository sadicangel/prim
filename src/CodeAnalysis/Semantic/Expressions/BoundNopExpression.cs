using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundNopExpression(SyntaxNode Syntax, TypeSymbol Type)
    : BoundExpression(BoundKind.NopExpression, Syntax, Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => [];
}
