using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundPropertyDeclaration(
    SyntaxNode Syntax,
    PropertySymbol PropertySymbol,
    BoundExpression Expression)
    : BoundMemberDeclaration(BoundKind.PropertyDeclaration, Syntax, PropertySymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return PropertySymbol;
        yield return Expression;
    }
}
