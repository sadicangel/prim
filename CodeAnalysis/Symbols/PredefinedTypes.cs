using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeAnalysis.Symbols;

internal static class PredefinedTypes
{
    public static readonly TypeSymbol Never = new(PredefinedTypeNames.Never);

    public static readonly TypeSymbol Any = new(PredefinedTypeNames.Any);

    public static readonly TypeSymbol Void = new(PredefinedTypeNames.Void);

    public static readonly TypeSymbol Type = new(PredefinedTypeNames.Type);

    public static readonly TypeSymbol Bool = new(PredefinedTypeNames.Bool);

    public static readonly TypeSymbol I8 = new(PredefinedTypeNames.I8);
    public static readonly TypeSymbol I16 = new(PredefinedTypeNames.I16);
    public static readonly TypeSymbol I32 = new(PredefinedTypeNames.I32);
    public static readonly TypeSymbol I64 = new(PredefinedTypeNames.I64);
    public static readonly TypeSymbol ISize = new(PredefinedTypeNames.ISize);

    public static readonly TypeSymbol U8 = new(PredefinedTypeNames.U8);
    public static readonly TypeSymbol U16 = new(PredefinedTypeNames.U16);
    public static readonly TypeSymbol U32 = new(PredefinedTypeNames.U32);
    public static readonly TypeSymbol U64 = new(PredefinedTypeNames.U64);
    public static readonly TypeSymbol USize = new(PredefinedTypeNames.USize);

    public static readonly TypeSymbol F32 = new(PredefinedTypeNames.F32);
    public static readonly TypeSymbol F64 = new(PredefinedTypeNames.F64);

    public static readonly TypeSymbol Str = new(PredefinedTypeNames.Str);

    public static readonly TypeSymbol Func = new(PredefinedTypeNames.Func);

    public static IReadOnlyList<TypeSymbol> All { get; } = typeof(PredefinedTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(TypeSymbol))
        .Select(f => (TypeSymbol)f.GetValue(null)!)
        .ToArray();

    public static bool TryLookup(string name, [MaybeNullWhen(false)] out TypeSymbol type)
    {
        type = All.SingleOrDefault(t => t.Name == name);
        return type is not null;
    }
}
