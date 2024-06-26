﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundNeverExpression BindEmptyExpression(EmptyExpressionSyntax syntax, BinderContext context)
    {
        return new BoundNeverExpression(syntax);
    }
}
