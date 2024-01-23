using System.Reflection;

namespace CodeAnalysis.Symbols;

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
