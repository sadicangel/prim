using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static VariableValue EvaluateLiteralExpression(BoundLiteralExpression node, InterpreterContext context)
    {
        _ = context;
        return node.BoundKind switch
        {
            BoundKind.I32LiteralExpression => new VariableValue(PredefinedTypes.I32, (int)node.Value),
            BoundKind.U32LiteralExpression => new VariableValue(PredefinedTypes.U32, (uint)node.Value),
            BoundKind.I64LiteralExpression => new VariableValue(PredefinedTypes.I64, (long)node.Value),
            BoundKind.U64LiteralExpression => new VariableValue(PredefinedTypes.U64, (ulong)node.Value),
            BoundKind.F32LiteralExpression => new VariableValue(PredefinedTypes.F32, (float)node.Value),
            BoundKind.F64LiteralExpression => new VariableValue(PredefinedTypes.F64, (double)node.Value),
            BoundKind.StrLiteralExpression => new VariableValue(PredefinedTypes.Str, (string)node.Value),
            BoundKind.TrueLiteralExpression => VariableValue.True,
            BoundKind.FalseLiteralExpression => VariableValue.False,
            BoundKind.NullLiteralExpression => VariableValue.Unit,
            _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'")
        };
    }
}
