using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundLambdaExpression(
    SyntaxNode Syntax,
    LambdaTypeSymbol LambdaType,
    ImmutableArray<VariableSymbol> Parameters,
    BoundExpression Body)
    : BoundExpression(BoundKind.LambdaExpression, Syntax, LambdaType)
{
    public override IEnumerable<ITreeNode> Children()
    {
        yield return LambdaType;
        foreach (var parameter in Parameters)
            yield return parameter;
        yield return Body;
    }
}
