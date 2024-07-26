﻿using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundArrayInitExpression LowerArrayInitExpression(BoundArrayInitExpression node, LowererContext context)
    {
        var elements = LowerList(node.Elements, context, LowerExpression);
        if (elements is null)
            return node;

        return node with { Elements = new(elements) };
    }
}