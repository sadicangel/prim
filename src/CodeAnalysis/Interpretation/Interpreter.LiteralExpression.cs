using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static LiteralValue EvaluateLiteralExpression(BoundLiteralExpression node, InterpreterContext context)
    {
        _ = context;
        var scope = GlobalEvaluatedScope.Instance;
        return node.Type switch
        {
            PrimType when node.Type == PredefinedTypes.I32 => new LiteralValue(scope.I32, node.Type, (int)node.Value),
            PrimType when node.Type == PredefinedTypes.U32 => new LiteralValue(scope.U32, node.Type, (uint)node.Value),
            PrimType when node.Type == PredefinedTypes.I64 => new LiteralValue(scope.I64, node.Type, (long)node.Value),
            PrimType when node.Type == PredefinedTypes.U64 => new LiteralValue(scope.U64, node.Type, (ulong)node.Value),
            PrimType when node.Type == PredefinedTypes.F32 => new LiteralValue(scope.F32, node.Type, (float)node.Value),
            PrimType when node.Type == PredefinedTypes.F64 => new LiteralValue(scope.F64, node.Type, (double)node.Value),
            PrimType when node.Type == PredefinedTypes.Str => new LiteralValue(scope.Str, node.Type, (string)node.Value),
            PrimType when node.Type == PredefinedTypes.Bool => new LiteralValue(scope.Bool, node.Type, (bool)node.Value),
            PrimType when node.Type == PredefinedTypes.Unit => new LiteralValue(scope.Unit, node.Type, node.Value),
            _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'")
        };
    }
}
