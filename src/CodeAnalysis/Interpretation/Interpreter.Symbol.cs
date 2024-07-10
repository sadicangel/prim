using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateSymbol(Symbol? symbol, InterpreterContext context)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        // TODO: Maybe have a bool prop that stores if symbol was defined in code?
        if (context.EvaluatedScope.TryLookup(symbol, out var value))
            return value;

        if (symbol is not TypeSymbol { Type: PrimType type })
            throw new UnreachableException($"Unexpected symbol '{symbol}'");


        return type switch
        {
            OptionType option => new OptionValue(option),
            _ => throw new UnreachableException($"Unexpected type {type}")
        };
    }
}
