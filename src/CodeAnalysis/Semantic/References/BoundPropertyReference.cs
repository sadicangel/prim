using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.References;

internal sealed record class BoundPropertyReference(
    SyntaxNode Syntax,
    BoundExpression? Receiver,
    PropertySymbol Property)
    : BoundReference(BoundKind.PropertyReference, Syntax, Property)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        if (Receiver is not null)
        {
            yield return Receiver;
        }
    }
}
