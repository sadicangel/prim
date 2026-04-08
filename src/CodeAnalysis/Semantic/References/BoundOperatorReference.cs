using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.References;

internal sealed record class BoundOperatorReference(SyntaxNode Syntax, OperatorSymbol Operator)
    : BoundReference(BoundKind.OperatorReference, Syntax, Operator)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => [];
}
