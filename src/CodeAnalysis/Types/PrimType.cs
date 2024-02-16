using CodeAnalysis.Operators;
using CodeAnalysis.Types.Members;
using System.Diagnostics;

namespace CodeAnalysis.Types;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract record class PrimType(string Name)
{
    public List<Property> Properties { get; init; } = [];
    public List<Operator> Operators { get; init; } = [];

    private string GetDebuggerDisplay() => $"{Name}: {PredefinedSymbolNames.Type}";

    public virtual bool Equals(PrimType? other) => other is not null && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public sealed override string ToString() => Name;

    public Conversion GetConversion(PrimType target)
    {
        if (this == PredefinedTypes.Never || target == PredefinedTypes.Never)
            return Conversion.None();

        if (this == target)
            return Conversion.Identity();

        if (target == PredefinedTypes.Any)
            return Conversion.Implicit(new UnaryOperator(OperatorKind.ImplicitConversion, this, target));

        var @operator = (UnaryOperator?)Operators
            .Find(op => op.OperatorKind is OperatorKind.ImplicitConversion or OperatorKind.ExplicitConversion && op.ResultType == target);

        if (@operator is not null)
            return @operator.OperatorKind is OperatorKind.ImplicitConversion
                ? Conversion.Implicit(@operator)
                : Conversion.Explicit(@operator);

        return Conversion.None();
    }
}

public enum ConversionType { None, Identity, Implicit, Explicit }

public readonly record struct Conversion
{
    private static readonly Conversion None_ = default;
    private static readonly Conversion Identity_ = new() { Type = ConversionType.Identity };

    public ConversionType Type { get; init; }
    public UnaryOperator? Operator { get; init; }

    public static Conversion None() => None_;
    public static Conversion Identity() => Identity_;
    public static Conversion Implicit(UnaryOperator @operator) => new() { Type = ConversionType.Implicit, Operator = @operator };
    public static Conversion Explicit(UnaryOperator @operator) => new() { Type = ConversionType.Explicit, Operator = @operator };

    public T Match<T>(Func<T> None, Func<T> Identity, Func<UnaryOperator, T> Implicit, Func<UnaryOperator, T> Explicit)
    {
        return Type switch
        {
            ConversionType.None => None(),
            ConversionType.Identity => Identity(),
            ConversionType.Implicit => Implicit(Operator!),
            ConversionType.Explicit => Explicit(Operator!),
            _ => throw new UnreachableException($"Unexpected {nameof(ConversionType)} '{Type}'")
        };
    }
}
