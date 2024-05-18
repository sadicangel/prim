using CodeAnalysis.Types;
using System.Diagnostics;

namespace CodeAnalysis;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class Symbol(string Name, PrimType Type)
{
    public bool IsMutable { get; init; }

    private string GetDebuggerDisplay() => $"{Name}: {Type}";

    public override string ToString() => Name;
}