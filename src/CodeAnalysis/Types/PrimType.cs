using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Types;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract record class PrimType(string Name)
{
    private readonly List<Member> _members = [];

    public bool IsAny { get => this == PredefinedTypes.Any; }
    public bool IsNever { get => this == PredefinedTypes.Never; }
    public bool IsOption { get => this is OptionType; }
    public bool IsUnknown { get => this == PredefinedTypes.Unknown; }
    public bool IsPredefined { get => PredefinedTypeNames.All.Contains(Name); }

    public IReadOnlyList<Member> Members => _members;

    private string GetDebuggerDisplay() => $"{Name}: {PredefinedTypeNames.Type}";

    public virtual bool Equals(PrimType? other) => other is not null && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public sealed override string ToString() => Name;

    internal bool IsCoercibleTo(PrimType type) => IsCoercibleTo(type, out _);
    internal bool IsCoercibleTo(PrimType type, out Conversion? conversion)
    {
        conversion = null;
        if (type.IsAny || this == type)
        {
            return true;
        }

        conversion = GetConversion(this, type) ?? type.GetConversion(this, type);

        return conversion is not null && conversion.IsImplicit;
    }

    internal bool IsConvertibleTo(PrimType type) => IsConvertibleTo(type, out _);
    internal bool IsConvertibleTo(PrimType type, out Conversion? conversion)
    {
        conversion = null;
        if (type.IsAny || this == type)
        {
            return true;
        }

        conversion = GetConversion(this, type) ?? type.GetConversion(this, type);

        return conversion is not null;
    }

    internal List<Member> GetMembers(ReadOnlySpan<char> name)
    {
        var list = new List<Member>();
        foreach (var member in _members)
        {
            var index = member.Name.IndexOf('<');
            var memberName = index >= 0 ? member.Name.AsSpan(0, index) : member.Name;
            if (memberName.Equals(name, StringComparison.Ordinal))
                list.Add(member);
        }
        return list;
    }

    internal bool AddProperty(string name, PrimType type, bool isReadonly)
    {
        if (GetProperty(name) is not null) return false;
        // TODO: Allow static properties.
        _members.Add(new Property(name, type, this, isReadonly, IsStatic: false));
        return true;
    }

    internal Property? GetProperty(ReadOnlySpan<char> name)
    {
        foreach (var property in _members.OfType<Property>())
        {
            if (name.Equals(property.Name, StringComparison.Ordinal))
                return property;
        }
        return null;
    }

    internal bool AddMethod(string name, FunctionType type)
    {
        if (GetMethod(name, type) is not null) return false;
        // TODO: Allow static methods.
        _members.Add(new Method(name, type, this, IsStatic: false));
        return true;
    }

    internal Method? GetMethod(ReadOnlySpan<char> name, FunctionType type)
        => GetMethod(SyntaxFacts.GetMethodName(name, type));

    internal Method? GetMethod(string name) =>
        _members.OfType<Method>().SingleOrDefault(m => m.Name == name);

    internal bool AddOperator(SyntaxKind operatorKind, FunctionType type)
    {
        if (GetOperator(operatorKind, type) is not null) return false;
        _members.Add(new Operator(operatorKind, type, this));
        return true;
    }

    internal Operator? GetOperator(SyntaxKind operatorKind, FunctionType type)
    {
        foreach (var @operator in _members.OfType<Operator>())
        {
            if (@operator.OperatorKind == operatorKind && @operator.Type == type)
                return @operator;
        }
        return null;
    }

    internal List<Operator> GetOperators(SyntaxKind operatorKind, Func<FunctionType, bool>? filter = null)
    {
        var operators = _members.OfType<Operator>()
            .Where(o => o.OperatorKind == operatorKind);

        if (filter is not null)
            operators = operators.Where(o => filter(o.Type));

        return operators.ToList();
    }

    internal List<Operator> GetUnaryOperators(SyntaxKind operatorKind, PrimType operandType, PrimType? resultType = null)
    {
        var operators = _members.OfType<Operator>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.Type.Parameters.Count == 1)
            .Where(o => operandType.IsCoercibleTo(o.Type.Parameters[0].Type))
            .Where(o => resultType is null || resultType.IsCoercibleTo(o.Type.ReturnType))
            .ToList();

        if (operators.Count > 1)
        {
            var @operator = operators.SingleOrDefault(o =>
                o.OperatorKind == operatorKind &&
                o.Type.Parameters.Count == 1 &&
                o.Type.Parameters[0].Type == operandType);

            if (@operator is not null)
                return [@operator];
        }

        return operators;
    }

    internal List<Operator> GetBinaryOperators(SyntaxKind operatorKind, PrimType leftType, PrimType rightType, PrimType? resultType = null)
    {
        var operators = _members.OfType<Operator>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.Type.Parameters.Count == 2)
            .Where(o => leftType.IsCoercibleTo(o.Type.Parameters[0].Type))
            .Where(o => rightType.IsCoercibleTo(o.Type.Parameters[1].Type))
            .Where(o => resultType is null || resultType.IsCoercibleTo(o.Type.ReturnType))
            .ToList();

        if (operators.Count > 1)
        {
            var @operator = operators.SingleOrDefault(o =>
                o.OperatorKind == operatorKind &&
                o.Type.Parameters.Count == 2 &&
                o.Type.Parameters[0].Type == leftType &&
                o.Type.Parameters[1].Type == rightType);

            if (@operator is not null)
                return [@operator];
        }

        return operators;
    }

    internal bool AddConversion(SyntaxKind conversionKind, FunctionType type)
    {
        if (GetConversion(type) is not null) return false;
        _members.Add(new Conversion(conversionKind, type, this));
        return true;
    }

    internal Conversion? GetConversion(FunctionType type)
    {
        return _members.OfType<Conversion>()
            .Where(x => x.Type == type)
            .SingleOrDefault();
    }

    internal Conversion? GetConversion(PrimType sourceType, PrimType targetType)
    {
        return _members.OfType<Conversion>()
            .Where(x => x.Type.Parameters[0].Type.IsAny || x.Type.Parameters[0].Type == sourceType)
            .Where(x => x.Type.ReturnType.IsAny || x.Type.ReturnType == targetType)
            .SingleOrDefault();
    }
}
