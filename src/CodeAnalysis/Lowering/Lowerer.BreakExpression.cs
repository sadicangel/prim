﻿using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;

partial class Lowerer
{
    private static BoundGotoExpression LowerBreakExpression(BoundBreakExpression node, LowererContext context)
    {
        _ = context;
        return new BoundGotoExpression(node.Syntax, node.LabelSymbol, node.Expression);
    }
}