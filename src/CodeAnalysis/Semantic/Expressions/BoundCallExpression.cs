using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundCallExpression(
    SyntaxNode Syntax,
    BoundExpression Callee,
    ImmutableArray<BoundExpression> Arguments,
    TypeSymbol Type)
    : BoundExpression(BoundKind.CallExpression, Syntax, Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        yield return Callee;
        foreach (var argument in Arguments)
            yield return argument;
    }
}
