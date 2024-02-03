using CodeAnalysis.Types;
using System.Diagnostics;

namespace CodeAnalysis.Binding;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal sealed record class Symbol(
    string Name,
    PrimType Type,
    bool IsMutable = false
)
{
    private string GetDebuggerDisplay() => $"{Name}: {Type.Name}";
}