using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundNeverExpression(SyntaxNode Syntax)
    : BoundExpression(BoundKind.NeverExpression, Syntax, PredefinedSymbols.Never)
{
    public override IEnumerable<BoundNode> Children() => [];
}
