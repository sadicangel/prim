using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructExpression(
     SyntaxNode SyntaxNode,
     StructSymbol StructSymbol,
     BoundList<BoundPropertyExpression> Properties)
    : BoundExpression(BoundKind.StructExpression, SyntaxNode, StructSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return StructSymbol;
        foreach (var property in Properties)
            yield return property;
    }
}
