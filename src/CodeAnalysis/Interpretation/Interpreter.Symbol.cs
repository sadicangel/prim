using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateSymbol(Symbol? symbol, InterpreterContext context)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        // TODO: Maybe have a bool prop that stores if symbol was defined in code?
        if (context.EvaluatedScope.TryLookup(symbol, out var value))
            return value;

        if (symbol is not TypeSymbol { Type: TypeSymbol type })
            throw new UnreachableException($"Unexpected symbol '{symbol}'");


        return type switch
        {
            _ => throw new UnreachableException($"Unexpected type {type}")
        };
    }
}
