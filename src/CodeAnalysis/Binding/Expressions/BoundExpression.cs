using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.ConstFolding;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundExpression(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    TypeSymbol Type)
    : BoundNode(BoundKind, Syntax)
{
    private object? _constValue;
    public object? ConstValue { get => _constValue ??= ConstFolder.FoldExpression(this); }
}
