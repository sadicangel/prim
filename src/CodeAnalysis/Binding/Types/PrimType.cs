using System.Diagnostics;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Types;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal abstract record class PrimType(string Name)
{
    private readonly List<Member> _members = [];

    public bool IsAny { get => this == PredefinedTypes.Any; }
    public bool IsNever { get => this == PredefinedTypes.Never; }
    public bool IsUnknown { get => this == PredefinedTypes.Unknown; }

    private string GetDebuggerDisplay() => $"{Name}: {PredefinedTypeNames.Type}";

    public virtual bool Equals(PrimType? other) => other is not null && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public sealed override string ToString() => Name;

    public bool AddProperty(Property property)
    {
        if (GetProperty(property.Name) is not null) return false;
        _members.Add(property);
        return true;
    }

    public Property? GetProperty(ReadOnlySpan<char> name)
    {
        foreach (var property in _members.OfType<Property>())
        {
            if (name.Equals(property.Name, StringComparison.Ordinal))
                return property;
        }
        return null;
    }

    public bool AddMethod(Method method)
    {
        if (GetMethod(method.Name, method.Type) is not null) return false;
        _members.Add(method);
        return true;
    }

    public Method? GetMethod(ReadOnlySpan<char> name, FunctionType type)
    {
        foreach (var method in _members.OfType<Method>())
        {
            if (name.Equals(method.Name, StringComparison.Ordinal) && method.Type == type)
                return method;
        }
        return null;
    }

    public bool AddOperator(Operator @operator)
    {
        if (GetOperator(@operator.OperatorKind, @operator.Type) is not null) return false;
        _members.Add(@operator);
        return true;
    }

    public Operator? GetOperator(SyntaxKind operatorKind, FunctionType type)
    {
        foreach (var @operator in _members.OfType<Operator>())
        {
            if (@operator.OperatorKind == operatorKind && @operator.Type == type)
                return @operator;
        }
        return null;
    }

    public List<Operator> GetUnaryOperators(SyntaxKind operatorKind, PrimType operandType, PrimType resultType)
    {
        return _members.OfType<Operator>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.Type.Parameters.Count == 1)
            .Where(o => o.Type.Parameters[0].Type.IsAny || o.Type.Parameters[0].Type == operandType)
            .Where(o => resultType.IsAny || o.Type.ReturnType == resultType)
            .ToList();
    }

    public List<Operator> GetBinaryOperators(SyntaxKind operatorKind, PrimType leftType, PrimType rightType, PrimType resultType)
    {
        return _members.OfType<Operator>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.Type.Parameters.Count == 2)
            .Where(o => o.Type.Parameters[0].Type.IsAny || o.Type.Parameters[0].Type == leftType)
            .Where(o => o.Type.Parameters[1].Type.IsAny || o.Type.Parameters[1].Type == rightType)
            .Where(o => resultType.IsAny || o.Type.ReturnType == resultType)
            .ToList();
    }

    public bool AddConversion(Conversion conversion)
    {
        if (GetConversion(conversion.Type) is not null) return false;
        _members.Add(conversion);
        return true;
    }

    public Conversion? GetConversion(FunctionType type)
    {
        return _members.OfType<Conversion>()
            .Where(x => x.Type == type)
            .SingleOrDefault();
    }

    public Conversion? GetConversion(PrimType sourceType, PrimType targetType)
    {
        return _members.OfType<Conversion>()
            .Where(x => x.Type.Parameters[0].Type.IsAny || x.Type.Parameters[0].Type == sourceType)
            .Where(x => x.Type.ReturnType.IsAny || x.Type.ReturnType == targetType)
            .SingleOrDefault();
    }
}
