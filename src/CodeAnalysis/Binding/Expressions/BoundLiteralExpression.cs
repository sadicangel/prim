using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLiteralExpression(BoundKind BoundKind, SyntaxNode SyntaxNode, PrimType Type, object? Value)
    : BoundExpression(BoundKind, SyntaxNode, Type)
{
    public override IEnumerable<BoundNode> Children() => [];
}
