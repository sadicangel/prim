﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static BoundExpression BindInitValueExpression(InitValueExpressionSyntax syntax, Context context)
    {
        var expression = BindExpression(syntax.Expression, context);
        return expression;
    }
}
