using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructInitExpression(
     SyntaxNode Syntax,
     TypeSymbol TypeSymbol,
     BoundList<BoundPropertyInitExpression> Properties)
    : BoundExpression(BoundKind.StructInitExpression, Syntax, TypeSymbol)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return TypeSymbol;
        foreach (var property in Properties)
            yield return property;
    }
}
