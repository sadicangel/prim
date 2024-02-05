using System.Diagnostics;

namespace CodeAnalysis.Types;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class Symbol(
    string Name,
    PrimType Type,
    bool IsMutable = false
)
{
    private string GetDebuggerDisplay() => $"{Name}: {Type.Name}";
}