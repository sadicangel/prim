using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundLocalReference(SyntaxNode Syntax, Symbol Symbol)
    : BoundExpression(BoundKind.LocalReference, Syntax, Symbol.Type)
{
    /// <inheritdoc />
    public override IEnumerable<ITreeNode> Children()
    {
        yield return Symbol;
    }
}
