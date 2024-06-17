using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructExpression(
     SyntaxNode Syntax,
     StructSymbol StructSymbol,
     BoundList<BoundPropertyExpression> Properties)
    : BoundExpression(BoundKind.StructExpression, Syntax, StructSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return StructSymbol;
        foreach (var property in Properties)
            yield return property;
    }
}
