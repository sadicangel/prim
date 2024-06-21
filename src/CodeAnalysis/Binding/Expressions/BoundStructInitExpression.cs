using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructInitExpression(
     SyntaxNode Syntax,
     StructSymbol StructSymbol,
     BoundList<BoundPropertyInitExpression> Properties)
    : BoundExpression(BoundKind.StructInitExpression, Syntax, StructSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return StructSymbol;
        foreach (var property in Properties)
            yield return property;
    }
}
