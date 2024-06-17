using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundNeverExpression(SyntaxNode Syntax)
    : BoundExpression(BoundKind.NeverExpression, Syntax, PredefinedTypes.Never)
{
    public override IEnumerable<BoundNode> Children() => [];
}
