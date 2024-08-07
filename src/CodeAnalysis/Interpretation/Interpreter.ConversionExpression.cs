﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateConversionExpression(BoundConversionExpression node, Context context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        var conversion = expression.Get<LambdaValue>(node.ConversionSymbol);
        var value = conversion.Invoke(expression);
        return value;
    }
}
