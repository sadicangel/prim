using System.Diagnostics;
using CodeAnalysis.Binding.Types.Metadata;

namespace CodeAnalysis.Binding.Types;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract record class PrimType(string Name)
{
    public bool IsNever { get => this == PredefinedTypes.Never; }

    public ReadOnlyList<Property> Properties { get; init; } = [];

    private string GetDebuggerDisplay() => $"{Name}: {PredefinedTypeNames.Type}";

    public virtual bool Equals(PrimType? other) => other is not null && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public sealed override string ToString() => Name;
}
