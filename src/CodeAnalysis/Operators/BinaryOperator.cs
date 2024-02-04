using CodeAnalysis.Types;
using System.Diagnostics;

namespace CodeAnalysis.Operators;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class BinaryOperator(
    OperatorKind OperatorKind,
    PrimType LeftType,
    PrimType RightType,
    PrimType ResultType
)
    : Operator(OperatorKind, ResultType)
{
    private string GetDebuggerDisplay() => $"operator {OperatorKind}: (a: {LeftType.Name}, b: {RightType.Name}) -> {ResultType.Name}";
}