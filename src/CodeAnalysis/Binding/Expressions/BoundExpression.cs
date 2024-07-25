using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.ConstantFolding;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundExpression(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    TypeSymbol Type)
    : BoundNode(BoundKind, Syntax)
{
    private object? _constValue;
    public object? ConstantValue { get => _constValue ??= ConstantFolder.FoldExpression(this); }
}
