using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static LiteralValue EvaluateLiteralExpression(BoundLiteralExpression node, Context context)
    {
        _ = context;
        var scope = GlobalEvaluatedScope.Instance;
        return node.Type switch
        {
            TypeSymbol when node.Type == PredefinedSymbols.I32 => new LiteralValue(scope.I32, (int)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.U32 => new LiteralValue(scope.U32, (uint)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.I64 => new LiteralValue(scope.I64, (long)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.U64 => new LiteralValue(scope.U64, (ulong)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.F32 => new LiteralValue(scope.F32, (float)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.F64 => new LiteralValue(scope.F64, (double)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.Str => new LiteralValue(scope.Str, (string)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.Bool => new LiteralValue(scope.Bool, (bool)node.Value),
            TypeSymbol when node.Type == PredefinedSymbols.Unit => new LiteralValue(scope.Unit, node.Value),
            _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'")
        };
    }
}
