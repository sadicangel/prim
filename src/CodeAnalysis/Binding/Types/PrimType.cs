using System.Diagnostics;
using CodeAnalysis.Binding.Types.Metadata;

namespace CodeAnalysis.Binding.Types;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal abstract record class PrimType(string Name)
{
    public bool IsAny { get => this == PredefinedTypes.Any; }
    public bool IsNever { get => this == PredefinedTypes.Never; }
    public bool IsUnknown { get => this == PredefinedTypes.Unknown; }

    public ReadOnlyList<Property> Properties { get; init; } = [];
    public ReadOnlyList<Method> Methods { get; init; } = [];
    public ReadOnlyList<Operator> Operators { get; init; } = [];

    private string GetDebuggerDisplay() => $"{Name}: {PredefinedTypeNames.Type}";

    public virtual bool Equals(PrimType? other) => other is not null && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public sealed override string ToString() => Name;

    public Property? GetProperty(ReadOnlySpan<char> name)
    {
        for (var i = 0; i < Properties.Count; ++i)
        {
            var property = Properties[i];
            if (name.Equals(property.Name, StringComparison.Ordinal))
                return property;
        }
        return null;
    }

    public List<Operator> GetUnaryOperators(string name, PrimType operandType, PrimType resultType)
    {
        return Operators
            .Where(o => o.Name == name)
            .Where(o => o.Type.Parameters.Count == 1)
            .Where(o => o.Type.Parameters[0].Type == operandType)
            .Where(o => resultType.IsAny || o.Type.ReturnType == resultType)
            .ToList();
    }

    public List<Operator> GetBinaryOperators(string name, PrimType leftType, PrimType rightType, PrimType resultType)
    {
        return Operators
            .Where(o => o.Name == name)
            .Where(o => o.Type.Parameters.Count == 2)
            .Where(o => o.Type.Parameters[0].Type == leftType)
            .Where(o => o.Type.Parameters[1].Type == rightType)
            .Where(o => resultType.IsAny || o.Type.ReturnType == resultType)
            .ToList();
    }
}
