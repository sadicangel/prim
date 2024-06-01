﻿using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundBlockExpression(
    SyntaxNode SyntaxNode,
    PrimType Type,
    BoundList<BoundExpression> Expressions)
    : BoundExpression(BoundKind.BlockExpression, SyntaxNode, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var expression in Expressions)
            yield return expression;
    }
}
