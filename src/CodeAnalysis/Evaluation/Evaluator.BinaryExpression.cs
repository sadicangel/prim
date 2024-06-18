﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Evaluation.Values;

namespace CodeAnalysis.Evaluation;
partial class Evaluator
{
    private static PrimValue EvaluateBinaryExpression(BoundBinaryExpression node, EvaluatorContext context)
    {
        if (node.OperatorSymbol.ContainingSymbol is StructSymbol structSymbol)
        {
            var structValue = (StructValue)context.EvaluatedScope.Lookup(structSymbol);
            var functionValue = structValue.GetOperator(node.OperatorSymbol);

            var left = EvaluateExpression(node.Left, context);
            var right = EvaluateExpression(node.Right, context);

            return functionValue.Invoke(left, right);
        }

        return node.OperatorSymbol.Operator.ContainingType switch
        {
            _ => throw new NotImplementedException($"BinaryExpression for '{node.OperatorSymbol.Operator.ContainingType}'")
        };
    }
}
