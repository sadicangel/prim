using CodeAnalysis.Types;
using System.Diagnostics;

namespace CodeAnalysis;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class Symbol(
    string Name,
    PrimType Type,
    bool IsMutable = false
)
{
    private string GetDebuggerDisplay() => $"{Name}: {Type}";

    public override string ToString() => Name;
}