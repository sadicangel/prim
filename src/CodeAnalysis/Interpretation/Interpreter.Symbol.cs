using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateSymbol(Symbol symbol, InterpreterContext context)
    {
        if (symbol is not TypeSymbol type)
            throw new UnreachableException($"Unexpected symbol '{symbol}'");

        return type switch
        {
            ArrayTypeSymbol => throw new NotImplementedException(type.GetType().Name),
            LambdaTypeSymbol => throw new NotImplementedException(type.GetType().Name),
            OptionTypeSymbol optionType => new OptionValue(optionType),
            StructTypeSymbol structType => context.EvaluatedScope.Lookup(structType),
            UnionTypeSymbol => throw new NotImplementedException(type.GetType().Name),
            _ => throw new UnreachableException($"Unexpected type '{type}'")
        };
    }
}
