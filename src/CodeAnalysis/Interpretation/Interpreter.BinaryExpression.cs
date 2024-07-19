﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateBinaryExpression(BoundBinaryExpression node, InterpreterContext context)
    {
        var left = EvaluateExpression(node.Left, context);
        var function = left.Get<LambdaValue>(node.MethodSymbol);
        var right = EvaluateExpression(node.Right, context);
        return function.Invoke(left, right);
    }
}
