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
        return node.BoundKind switch
        {
            BoundKind.I32LiteralExpression => new LiteralValue(GlobalEvaluatedScope.Instance.I32, PredefinedTypes.I32, (int)node.Value),
            BoundKind.U32LiteralExpression => new LiteralValue(GlobalEvaluatedScope.Instance.U32, PredefinedTypes.U32, (uint)node.Value),
            BoundKind.I64LiteralExpression => new LiteralValue(GlobalEvaluatedScope.Instance.I64, PredefinedTypes.I64, (long)node.Value),
            BoundKind.U64LiteralExpression => new LiteralValue(GlobalEvaluatedScope.Instance.U64, PredefinedTypes.U64, (ulong)node.Value),
            BoundKind.F32LiteralExpression => new LiteralValue(GlobalEvaluatedScope.Instance.F32, PredefinedTypes.F32, (float)node.Value),
            BoundKind.F64LiteralExpression => new LiteralValue(GlobalEvaluatedScope.Instance.F64, PredefinedTypes.F64, (double)node.Value),
            BoundKind.StrLiteralExpression => new LiteralValue(GlobalEvaluatedScope.Instance.Str, PredefinedTypes.Str, (string)node.Value),
            BoundKind.TrueLiteralExpression => LiteralValue.True,
            BoundKind.FalseLiteralExpression => LiteralValue.False,
            BoundKind.NullLiteralExpression => LiteralValue.Unit,
            _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'")
        };
    }
}
