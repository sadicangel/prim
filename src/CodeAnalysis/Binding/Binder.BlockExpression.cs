﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundBlockExpression BindBlockExpression(BlockExpressionSyntax syntax, BindingContext context)
    {
        PrimType type = PredefinedTypes.Unit;
        var expressions = new BoundList<BoundExpression>.Builder(syntax.Expressions.Count);
        foreach (var expressionSyntax in syntax.Expressions)
        {
            var expression = BindExpression(expressionSyntax, context);
            if (!type.IsNever)
                type = expression.Type;
            expressions.Add(expression);
        }
        return new BoundBlockExpression(syntax, type, expressions.ToBoundList());
    }
}
