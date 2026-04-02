using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.References;

internal sealed record class BoundVariableReference(SyntaxNode Syntax, VariableSymbol Variable)
    : BoundReference(BoundKind.VariableReference, Syntax, Variable)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => [];
}