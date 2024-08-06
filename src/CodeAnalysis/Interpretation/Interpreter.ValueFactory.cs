using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static class ValueFactory
    {
        // TODO: Make a "ctor" expression of some kind.
        public static PrimValue Create(TypeSymbol type, BoundExpression expression, Context context)
        {
            if (expression.Type == type)
            {
                return EvaluateExpression(expression, context);
            }

            return type switch
            {
                ArrayTypeSymbol => throw new NotSupportedException(nameof(ArrayTypeSymbol)),
                ErrorTypeSymbol errorType => new ErrorValue(errorType, EvaluateExpression(expression, context)),
                LambdaTypeSymbol lambdaType => new LambdaValue(lambdaType, FuncFactory.Create(lambdaType, expression, context)),
                OptionTypeSymbol optionType => new OptionValue(optionType, EvaluateExpression(expression, context)),
                PointerTypeSymbol => throw new NotSupportedException(nameof(PointerTypeSymbol)),
                StructTypeSymbol => throw new NotSupportedException(nameof(StructTypeSymbol)),
                UnionTypeSymbol unionType => new UnionValue(unionType, EvaluateExpression(expression, context)),
                _ => throw new UnreachableException($"Unexpected type '{type}'")
            };
        }
    }
}
