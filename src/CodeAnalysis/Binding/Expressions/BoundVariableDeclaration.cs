using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundVariableDeclaration(
    SyntaxNode SyntaxNode,
    VariableSymbol VariableSymbol,
    BoundExpression Expression)
    : BoundDeclaration(BoundKind.VariableDeclaration, SyntaxNode, VariableSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return VariableSymbol;
        yield return Expression;
    }
}
