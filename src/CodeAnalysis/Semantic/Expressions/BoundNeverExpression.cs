using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundNeverExpression(SyntaxNode Syntax, TypeSymbol Type)
    : BoundExpression(BoundKind.NeverExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children() => [];
}
