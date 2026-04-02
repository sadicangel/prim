using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.References;

internal abstract record class BoundReference(BoundKind BoundKind, SyntaxNode Syntax, Symbol Symbol)
    : BoundExpression(BoundKind, Syntax, Symbol.Type)
{
    public bool IsReadOnly => Symbol.Modifiers.HasFlag(Modifiers.ReadOnly);
}