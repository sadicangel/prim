using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundOperatorDeclaration(
    SyntaxNode Syntax,
    FunctionSymbol FunctionSymbol,
    BoundExpression Expression)
    : BoundMemberDeclaration(BoundKind.OperatorDeclaration, Syntax, FunctionSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return FunctionSymbol;
        yield return Expression;
    }
}
