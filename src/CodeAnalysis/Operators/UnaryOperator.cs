using CodeAnalysis.Types;
using System.Diagnostics;

namespace CodeAnalysis.Operators;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class UnaryOperator(
    UnaryOperatorKind OperatorKind,
    PrimType OperandType,
    PrimType ResultType
)
    : Operator(ResultType)
{
    private string GetDebuggerDisplay() => $"operator {OperatorKind}: (a: {OperandType.Name}) -> {ResultType.Name}";
}

public enum UnaryOperatorKind
{
    UnaryPlus,
    Negate,
    Increment,
    Decrement,
    Not,
    OnesComplement
}