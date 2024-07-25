﻿using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundUnaryExpression LowerUnaryExpression(BoundUnaryExpression node)
    {
        var operand = LowerExpression(node.Operand);
        if (ReferenceEquals(operand, node.Operand))
            return node;

        return node with { Operand = operand };
    }
}
