﻿using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundIndexReference(
    SyntaxNode Syntax,
    BoundExpression Expression,
    Symbol Symbol,
    BoundExpression Index,
    TypeSymbol Type)
    : BoundReference(BoundKind.IndexReference, Syntax, Symbol, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return Symbol;
        yield return Index;
        yield return Type;
    }
}
