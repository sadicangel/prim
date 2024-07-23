﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression Convert(BoundExpression expression, TypeSymbol type, bool isExplicit, BinderContext context)
    {
        if (expression.Type.IsNever)
        {
            return expression;
        }

        if (expression.Type.IsConvertibleTo(type, out var conversion))
        {
            if (conversion is null)
            {
                if (isExplicit)
                {
                    context.Diagnostics.ReportRedundantConversion(expression.Syntax.Location);
                }
                return expression;
            }

            if (!isExplicit && conversion.IsExplicitConversion)
            {
                context.Diagnostics.ReportInvalidImplicitConversion(expression.Syntax.Location, expression.Type.Name, type.Name);
                return new BoundNeverExpression(expression.Syntax);
            }

            return new BoundUnaryExpression(expression.Syntax, conversion, expression);
        }

        context.Diagnostics.ReportInvalidExpressionType(expression.Syntax.Location, type.Name, expression.Type.Name);
        return new BoundNeverExpression(expression.Syntax);
    }
}
