using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundNopExpression(
    SyntaxNode Syntax,
    TypeSymbol Type)
    : BoundExpression(BoundKind.NopExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children() => [];
}
