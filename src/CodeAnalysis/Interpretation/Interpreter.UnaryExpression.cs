using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateUnaryExpression(BoundUnaryExpression node, InterpreterContext context)
    {
        var operand = EvaluateExpression(node.Operand, context);
        var symbol = EvaluateSymbol(node.MethodSymbol.ContainingSymbol, context);
        var function = symbol.Get<FunctionValue>(node.MethodSymbol);
        var value = function.Invoke(operand);
        return value;
    }

    public static PrimValue EvaluateSymbol(Symbol? symbol, InterpreterContext context)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        switch (symbol)
        {
            case StructSymbol @struct:
                return context.EvaluatedScope.Lookup(symbol);
            case TypeSymbol type:
                return type.Type switch
                {
                    OptionType option => new OptionValue(option),
                    _ => throw new UnreachableException($"Unexpected type {type.Type}")
                };
            default:
                throw new UnreachableException($"Unexpected symbol '{symbol}'");
        }
    }
}
