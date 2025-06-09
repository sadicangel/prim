using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;
internal sealed record class BoundLambdaExpression(
    SyntaxNode Syntax,
    TypeSymbol Type,
    ImmutableArray<VariableSymbol> Parameters,
    BoundExpression Body)
    : BoundExpression(BoundKind.LambdaExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Body;
    }
}
