﻿using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static class ValueFactory
    {
        public static PrimValue Create(TypeSymbol type, BoundExpression value, InterpreterContext context)
        {
            return type switch
            {
                ArrayTypeSymbol => throw new NotImplementedException(type.GetType().Name),
                LambdaTypeSymbol => throw new NotImplementedException(type.GetType().Name),
                OptionTypeSymbol optionType => new OptionValue(optionType, EvaluateExpression(value, context)),
                StructTypeSymbol structType => context.EvaluatedScope.Lookup(structType),
                UnionTypeSymbol unionType => new UnionValue(unionType, EvaluateExpression(value, context)),
                _ => throw new UnreachableException($"Unexpected type '{type}'")
            };
        }
    }
}