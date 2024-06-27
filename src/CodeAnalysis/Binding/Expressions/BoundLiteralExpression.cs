using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLiteralExpression(SyntaxNode Syntax, PrimType Type, object Value)
    : BoundExpression(BoundKind.LiteralExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children() => [];
}
