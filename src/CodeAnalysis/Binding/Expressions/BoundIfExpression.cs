﻿using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundIfExpression(
    SyntaxNode Syntax,
    BoundExpression Condition,
    BoundExpression Then,
    BoundExpression Else,
    TypeSymbol Type)
    : BoundExpression(BoundKind.IfExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Condition;
        yield return Then;
        yield return Else;
    }
}
