using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundGlobalReference(SyntaxNode Syntax, Symbol Symbol)
    : BoundExpression(BoundKind.GlobalReference, Syntax, Symbol.Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => [];
}
