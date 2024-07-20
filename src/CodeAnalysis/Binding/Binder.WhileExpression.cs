﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindWhileExpression(WhileExpressionSyntax syntax, BinderContext context)
    {
        var condition = Coerce(BindExpression(syntax.Condition, context), PredefinedSymbols.Bool, context);
        if (condition.Type.IsNever)
        {
            return condition;
        }

        var body = BindExpression(syntax.Body, context);
        if (body.Type.IsNever)
        {
            return body;
        }

        return new BoundWhileExpression(syntax, condition, body);
    }
}
