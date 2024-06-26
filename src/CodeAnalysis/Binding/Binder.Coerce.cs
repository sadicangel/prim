﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression Coerce(BoundExpression expression, PrimType type, BinderContext context)
    {
        if (expression.Type.IsCoercibleTo(type, out var conversion))
        {
            if (conversion is null)
            {
                return expression;
            }

            var conversionSymbol = new ConversionSymbol(expression.Syntax, conversion);

            return new BoundConversionExpression(expression.Syntax, conversionSymbol, expression);
        }

        if (conversion?.IsExplicit is true)
        {
            context.Diagnostics.ReportInvalidImplicitConversion(expression.Syntax.Location, expression.Type.Name, type.Name);
            return new BoundNeverExpression(expression.Syntax);
        }

        context.Diagnostics.ReportInvalidExpressionType(expression.Syntax.Location, type.Name, expression.Type.Name);
        return new BoundNeverExpression(expression.Syntax);
    }
}
