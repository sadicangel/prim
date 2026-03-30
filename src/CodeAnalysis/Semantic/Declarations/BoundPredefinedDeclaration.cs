using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Semantic.Declarations;

internal sealed record class BoundPredefinedDeclaration(Symbol Symbol)
    : BoundDeclaration(BoundKind.PredefinedDeclaration, Symbol.Syntax, Symbol.Type)
{
    /// <inheritdoc />
    public override IEnumerable<ITreeNode> Children()
    {
        yield return Symbol;
    }
}
