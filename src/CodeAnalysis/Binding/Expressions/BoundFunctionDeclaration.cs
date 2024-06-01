using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundFunctionDeclaration(
    SyntaxNode SyntaxNode,
    FunctionSymbol Symbol,
    BoundExpression Expression)
    : BoundDeclaration(BoundKind.FunctionDeclaration, SyntaxNode, Symbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Symbol;
        yield return Expression;
    }
}
