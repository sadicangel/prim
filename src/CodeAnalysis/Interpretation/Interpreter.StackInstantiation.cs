using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateStackInstantiation(BoundStackInstantiation node, Context context) => node.Type switch
    {
        ArrayTypeSymbol => throw new NotSupportedException(nameof(ArrayTypeSymbol)),
        ErrorTypeSymbol errorType => new ErrorValue(errorType, EvaluateExpression(node.Expression, context)),
        LambdaTypeSymbol lambdaType => new LambdaValue(lambdaType, FuncFactory.Create(lambdaType, node.Expression, context)),
        OptionTypeSymbol optionType => new OptionValue(optionType, EvaluateExpression(node.Expression, context)),
        PointerTypeSymbol => throw new NotSupportedException(nameof(PointerTypeSymbol)),
        StructTypeSymbol => throw new NotSupportedException(nameof(StructTypeSymbol)),
        UnionTypeSymbol unionType => new UnionValue(unionType, EvaluateExpression(node.Expression, context)),
        _ => throw new UnreachableException($"Unexpected type '{node.Type}'")
    };
}
