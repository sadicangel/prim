﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression Coerce(BoundExpression expression, TypeSymbol type, Context context)
    {
        if (expression.Type.IsNever || expression.Type == type)
        {
            return expression;
        }

        if (expression.Type.IsCoercibleTo(type, out var conversion))
        {
            if (conversion is null)
            {
                return new BoundStackInstantiation(expression.Syntax, expression, type);
            }

            return new BoundConversionExpression(expression.Syntax, conversion, expression);
        }

        if (conversion?.IsExplicit is true)
        {
            context.Diagnostics.ReportInvalidImplicitConversion(expression.Syntax.Location, expression.Type.Name, type.Name);
            return new BoundNeverExpression(expression.Syntax, context.BoundScope.Never);
        }

        context.Diagnostics.ReportInvalidExpressionType(expression.Syntax.Location, type.Name, expression.Type.Name);
        return new BoundNeverExpression(expression.Syntax, context.BoundScope.Never);
    }
}
