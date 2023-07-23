using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal abstract record class BoundExpression(BoundNodeKind NodeKind, SyntaxNode Syntax, TypeSymbol Type) : BoundNode(NodeKind, Syntax)
{
    public virtual ConstantValue? ConstantValue { get; }
    public abstract T Accept<T>(IBoundExpressionVisitor<T> visitor);
    public override string ToString() => base.ToString();
}