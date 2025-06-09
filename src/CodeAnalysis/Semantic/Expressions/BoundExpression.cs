using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;
internal abstract record class BoundExpression(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    TypeSymbol Type)
    : BoundNode(BoundKind, Syntax)
{
    //public object? ConstantValue { get => field ??= ConstantFolder.FoldExpression(this); }
}
