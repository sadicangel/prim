using CodeAnalysis.Operators;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Types;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract record class PrimType(string Name)
{
    public List<Operator> Operators { get; init; } = [];

    private string GetDebuggerDisplay() => $"{Name}: {PredefinedTypeNames.Type}";

    public abstract bool IsAssignableFrom(PrimType source);

    // TODO: Override this with actual conversions.
    public bool IsConvertibleTo(PrimType target, [MaybeNullWhen(false)] out object conversion)
    {
        conversion = "identity";
        return this == target;
    }
}
