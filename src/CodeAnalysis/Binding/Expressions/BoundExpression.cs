using CodeAnalysis.ConstFolding;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundExpression(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    PrimType Type)
    : BoundNode(BoundKind, Syntax)
{
    private object? _constValue;
    public object? ConstValue { get => _constValue ??= ConstFolder.FoldExpression(this); }
}
