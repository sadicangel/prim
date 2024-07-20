using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static LiteralValue EvaluateLiteralExpression(BoundLiteralExpression node, InterpreterContext context)
    {
        _ = context;
        var scope = GlobalEvaluatedScope.Instance;
        return node.Type switch
        {
            TypeSymbol when node.Type == PredefinedSymbols.I32 => new LiteralValue(scope.I32, node.Type, (int)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.U32 => new LiteralValue(scope.U32, node.Type, (uint)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.I64 => new LiteralValue(scope.I64, node.Type, (long)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.U64 => new LiteralValue(scope.U64, node.Type, (ulong)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.F32 => new LiteralValue(scope.F32, node.Type, (float)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.F64 => new LiteralValue(scope.F64, node.Type, (double)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.Str => new LiteralValue(scope.Str, node.Type, (string)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.Bool => new LiteralValue(scope.Bool, node.Type, (bool)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.Unit => new LiteralValue(scope.Unit, node.Type, node.Value),
            _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'")
        };
    }
}
