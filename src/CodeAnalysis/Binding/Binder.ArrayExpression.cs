﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundArrayExpression BindArrayExpression(ArrayExpressionSyntax syntax, BinderContext context)
    {
        PrimType type = PredefinedTypes.Type;
        var expressions = new BoundList<BoundExpression>.Builder(syntax.Expressions.Count);
        foreach (var expressionSyntax in syntax.Expressions)
        {
            var expression = BindExpression(expressionSyntax, context);
            // TODO: Use type resolution here.
            if (!type.IsNever)
                type = expression.Type;
            expressions.Add(expression);
        }
        return new BoundArrayExpression(syntax, type, expressions.ToBoundList());
    }
}
