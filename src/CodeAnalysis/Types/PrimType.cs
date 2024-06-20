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
    public bool IsUnknown { get => this == PredefinedTypes.Unknown; }
    public bool IsPredefined { get => PredefinedTypeNames.All.Contains(Name); }

    private string GetDebuggerDisplay() => $"{Name}: {PredefinedTypeNames.Type}";

    public virtual bool Equals(PrimType? other) => other is not null && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public sealed override string ToString() => Name;

    internal bool AddProperty(string name, PrimType type, bool isReadonly)
    {
        if (GetProperty(name) is not null) return false;
        _members.Add(new Property(name, type, this, isReadonly));
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
        _members.Add(new Method(name, type, this));
        return true;
    }

    internal Method? GetMethod(ReadOnlySpan<char> name, FunctionType type)
    {
        foreach (var method in _members.OfType<Method>())
        {
            if (name.Equals(method.Name, StringComparison.Ordinal) && method.Type == type)
                return method;
        }
        return null;
    }

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

    internal List<Operator> GetUnaryOperators(SyntaxKind operatorKind, PrimType operandType, PrimType resultType)
    {
        var operators = _members.OfType<Operator>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.Type.Parameters.Count == 1)
            .Where(o => o.Type.Parameters[0].Type.IsAny || o.Type.Parameters[0].Type == operandType)
            .Where(o => resultType.IsAny || o.Type.ReturnType == resultType)
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

    internal List<Operator> GetBinaryOperators(SyntaxKind operatorKind, PrimType leftType, PrimType rightType, PrimType resultType)
    {
        var operators = _members.OfType<Operator>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.Type.Parameters.Count == 2)
            .Where(o => o.Type.Parameters[0].Type.IsAny || o.Type.Parameters[0].Type == leftType)
            .Where(o => o.Type.Parameters[1].Type.IsAny || o.Type.Parameters[1].Type == rightType)
            .Where(o => resultType.IsAny || o.Type.ReturnType == resultType)
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
