using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundReference(SyntaxNode Syntax, Symbol Symbol)
    : BoundExpression(BoundKind.Reference, Syntax, Symbol.Type)
{
    /// <inheritdoc />
    public override IEnumerable<ITreeNode> Children()
    {
        yield return Symbol;
    }
}
