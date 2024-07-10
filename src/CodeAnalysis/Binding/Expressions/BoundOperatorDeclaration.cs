using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundOperatorDeclaration(
    SyntaxNode Syntax,
    MethodSymbol MethodSymbol,
    BoundExpression Expression)
    : BoundMemberDeclaration(BoundKind.OperatorDeclaration, Syntax, MethodSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return MethodSymbol;
        yield return Expression;
    }
}
