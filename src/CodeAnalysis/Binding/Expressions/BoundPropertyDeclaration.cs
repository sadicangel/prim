using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundPropertyDeclaration(
    SyntaxNode SyntaxNode,
    PropertySymbol Symbol,
    BoundExpression Expression)
    : BoundDeclaration(BoundKind.PropertyDeclaration, SyntaxNode, Symbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Symbol;
        yield return Expression;
    }
}
