using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundNeverExpression(SyntaxNode Syntax, TypeSymbol Type)
    : BoundExpression(BoundKind.NeverExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children() => [];
}
