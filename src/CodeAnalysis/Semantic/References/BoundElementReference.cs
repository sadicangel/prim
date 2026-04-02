using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.References;

internal sealed record class BoundElementReference(
    SyntaxNode Syntax,
    BoundExpression Receiver,
    IndexerSymbol Indexer,
    BoundExpression Index)
    : BoundReference(BoundKind.ElementReference, Syntax, Indexer)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        yield return Receiver;
        yield return Index;
    }
}