using BindingFlags = System.Reflection.BindingFlags;

namespace CodeAnalysis.Binding.Symbols;

internal static class PredefinedSymbolNames
{
    public const string Any = "any";
    public const string Unknown = "unknown";
    public const string Never = "never";
    public const string Unit = "unit";
    public const string Type = "type";
    public const string Str = "str";
    public const string Bool = "bool";
    public const string I8 = "i8";
    public const string I16 = "i16";
    public const string I32 = "i32";
    public const string I64 = "i64";
    public const string I128 = "i128";
    public const string ISize = "isize";
    public const string U8 = "u8";
    public const string U16 = "u16";
    public const string U32 = "u32";
    public const string U64 = "u64";
    public const string U128 = "u128";
    public const string USize = "usize";
    public const string F16 = "f16";
    public const string F32 = "f32";
    public const string F64 = "f64";
    public const string F80 = "f80";
    public const string F128 = "f128";

    public static ReadOnlyList<string> All { get; } = new(typeof(PredefinedSymbolNames)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral)
        .Select(f => (string)f.GetValue(null)!)
        .ToArray());
}
