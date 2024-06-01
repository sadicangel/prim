using System.Diagnostics;
using System.Reflection;

namespace CodeAnalysis.Binding.Types;
internal static class PredefinedTypes
{
    public static readonly PredefinedType Any = new(PredefinedTypeNames.Any);
    public static readonly PredefinedType Unknown = new(PredefinedTypeNames.Unknown);
    public static readonly PredefinedType Never = new(PredefinedTypeNames.Never);
    public static readonly PredefinedType Unit = new(PredefinedTypeNames.Unit);
    public static readonly PredefinedType Type = new(PredefinedTypeNames.Type);
    public static readonly PredefinedType Str = new(PredefinedTypeNames.Str);
    public static readonly PredefinedType Bool = new(PredefinedTypeNames.Bool);
    public static readonly PredefinedType I8 = new(PredefinedTypeNames.I8);
    public static readonly PredefinedType I16 = new(PredefinedTypeNames.I16);
    public static readonly PredefinedType I32 = new(PredefinedTypeNames.I32);
    public static readonly PredefinedType I64 = new(PredefinedTypeNames.I64);
    public static readonly PredefinedType I128 = new(PredefinedTypeNames.I128);
    public static readonly PredefinedType ISize = new(PredefinedTypeNames.ISize);
    public static readonly PredefinedType U8 = new(PredefinedTypeNames.U8);
    public static readonly PredefinedType U16 = new(PredefinedTypeNames.U16);
    public static readonly PredefinedType U32 = new(PredefinedTypeNames.U32);
    public static readonly PredefinedType U64 = new(PredefinedTypeNames.U64);
    public static readonly PredefinedType U128 = new(PredefinedTypeNames.U128);
    public static readonly PredefinedType USize = new(PredefinedTypeNames.USize);
    public static readonly PredefinedType F16 = new(PredefinedTypeNames.F16);
    public static readonly PredefinedType F32 = new(PredefinedTypeNames.F32);
    public static readonly PredefinedType F64 = new(PredefinedTypeNames.F64);
    public static readonly PredefinedType F80 = new(PredefinedTypeNames.F80);
    public static readonly PredefinedType F128 = new(PredefinedTypeNames.F128);

    public static ReadOnlyList<PredefinedType> All { get; } = new(typeof(PredefinedTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(PredefinedType))
        .Select(f => (PredefinedType)f.GetValue(null)!)
        .ToArray());

    public static PredefinedType GetByName(ReadOnlySpan<char> typeName)
    {
        foreach (var type in All)
            if (typeName.Equals(type.Name, StringComparison.Ordinal))
                return type;

        throw new UnreachableException($"Unknown predefined type '{typeName}'");
    }
}
