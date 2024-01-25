using CodeAnalysis.Text;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeAnalysis;

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
        return type.Name is not null;
    }
}

internal static class PredefinedTypeNames
{
    public const string Never = "never";

    public const string Any = "any";

    public const string Void = "void";

    public const string Type = "type";

    public const string Bool = "bool";

    public const string I8 = "i8";
    public const string I16 = "i16";
    public const string I32 = "i32";
    public const string I64 = "i64";
    public const string ISize = "isize";

    public const string U8 = "u8";
    public const string U16 = "u16";
    public const string U32 = "u32";
    public const string U64 = "u64";
    public const string USize = "usize";

    public const string F32 = "f32";
    public const string F64 = "f64";

    public const string Str = "str";

    public const string Func = "func";

    public static IReadOnlyList<string> All { get; } = typeof(PredefinedTypeNames)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral && f.IsInitOnly)
        .Select(f => (string)f.GetValue(null)!)
        .ToArray();
}