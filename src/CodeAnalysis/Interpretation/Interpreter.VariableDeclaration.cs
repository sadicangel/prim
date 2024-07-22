﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static LiteralValue EvaluateVariableDeclaration(BoundVariableDeclaration node, InterpreterContext context)
    {
        var value = node.VariableSymbol.Type is LambdaTypeSymbol lambdaType
            ? new LambdaValue(lambdaType, FuncFactory.Create(lambdaType, node.Expression, context))
            : EvaluateExpression(node.Expression, context);
        context.EvaluatedScope.Declare(node.VariableSymbol, value);

        return PrimValue.Unit;
    }
}
