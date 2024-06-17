using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLiteralExpression(BoundKind BoundKind, SyntaxNode Syntax, PrimType Type, object? Value)
    : BoundExpression(BoundKind, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children() => [];
}
