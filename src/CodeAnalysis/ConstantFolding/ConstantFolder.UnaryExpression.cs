﻿using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation;

namespace CodeAnalysis.ConstantFolding;
partial class ConstantFolder
{
    private static object? FoldUnaryExpression(BoundUnaryExpression node)
    {
        var operand = node.Operand.ConstantValue;
        if (operand is null)
        {
            return null;
        }

        var typedValue = Interpreter.EvaluateUnaryExpression(
            node,
            new Interpreter.Context(new EvaluatedScope(node.Type.ContainingModule.Global), []));

        Debug.Assert(typedValue.Type == node.Type);

        return typedValue.Value;
    }
}

