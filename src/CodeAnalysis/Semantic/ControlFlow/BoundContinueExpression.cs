using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.ControlFlow;

internal sealed record class BoundContinueExpression(
    SyntaxNode Syntax,
    TypeSymbol Type)
    : BoundExpression(BoundKind.ContinueExpression, Syntax, Type)
{
    public override bool CanExitScope => true;

    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => [];
}
