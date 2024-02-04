using CodeAnalysis.Types;
using System.Diagnostics;

namespace CodeAnalysis.Operators;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class UnaryOperator(
    OperatorKind OperatorKind,
    PrimType OperandType,
    PrimType ResultType
)
    : Operator(OperatorKind, ResultType)
{
    private string GetDebuggerDisplay() => $"operator {OperatorKind}: (a: {OperandType.Name}) -> {ResultType.Name}";
}
