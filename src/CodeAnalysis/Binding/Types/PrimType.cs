using System.Diagnostics;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Types;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal abstract record class PrimType(string Name)
{
    public bool IsAny { get => this == PredefinedTypes.Any; }
    public bool IsNever { get => this == PredefinedTypes.Never; }
    public bool IsUnknown { get => this == PredefinedTypes.Unknown; }

    public List<Member> Members { get; init; } = [];

    private string GetDebuggerDisplay() => $"{Name}: {PredefinedTypeNames.Type}";

    public virtual bool Equals(PrimType? other) => other is not null && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public sealed override string ToString() => Name;

    public Property? GetProperty(ReadOnlySpan<char> name)
    {
        foreach (var property in Members.OfType<Property>())
        {
            if (name.Equals(property.Name, StringComparison.Ordinal))
                return property;
        }
        return null;
    }

    public Method? GetMethod(ReadOnlySpan<char> name, FunctionType type)
    {
        foreach (var method in Members.OfType<Method>())
        {
            if (name.Equals(method.Name, StringComparison.Ordinal) && method.Type == type)
                return method;
        }
        return null;
    }

    public Operator? GetOperator(SyntaxKind operatorKind, FunctionType type)
    {
        foreach (var @operator in Members.OfType<Operator>())
        {
            if (@operator.OperatorKind == operatorKind && @operator.Type == type)
                return @operator;
        }
        return null;
    }

    public List<Operator> GetUnaryOperators(SyntaxKind operatorKind, PrimType operandType, PrimType resultType)
    {
        return Members.OfType<Operator>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.Type.Parameters.Count == 1)
            .Where(o => o.Type.Parameters[0].Type == operandType)
            .Where(o => resultType.IsAny || o.Type.ReturnType == resultType)
            .ToList();
    }

    public List<Operator> GetBinaryOperators(SyntaxKind operatorKind, PrimType leftType, PrimType rightType, PrimType resultType)
    {
        return Members.OfType<Operator>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.Type.Parameters.Count == 2)
            .Where(o => o.Type.Parameters[0].Type == leftType)
            .Where(o => o.Type.Parameters[1].Type == rightType)
            .Where(o => resultType.IsAny || o.Type.ReturnType == resultType)
            .ToList();
    }
}
