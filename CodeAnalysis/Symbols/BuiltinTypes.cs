using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeAnalysis.Symbols;

internal static class BuiltinTypes
{
    private static readonly Lazy<ConcurrentDictionary<string, TypeSymbol>> TypeMap = new(() => new ConcurrentDictionary<string, TypeSymbol>(typeof(BuiltinTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(TypeSymbol))
        .Select(f => (TypeSymbol)f.GetValue(null)!)
        .ToDictionary(f => f.Name)));

    public static readonly TypeSymbol Never = new("never");

    public static readonly TypeSymbol Any = new("any");

    public static readonly TypeSymbol Void = new("void");

    public static readonly TypeSymbol Type = new("type");

    public static readonly TypeSymbol Bool = new("bool");

    public static readonly TypeSymbol I8 = new("i8");
    public static readonly TypeSymbol I16 = new("i16");
    public static readonly TypeSymbol I32 = new("i32");
    public static readonly TypeSymbol I64 = new("i64");

    public static readonly TypeSymbol U8 = new("u8");
    public static readonly TypeSymbol U16 = new("u16");
    public static readonly TypeSymbol U32 = new("u32");
    public static readonly TypeSymbol U64 = new("u64");

    public static readonly TypeSymbol F32 = new("f32");
    public static readonly TypeSymbol F64 = new("f64");

    public static readonly TypeSymbol Str = new("str");

    public static IEnumerable<TypeSymbol> All { get => TypeMap.Value.Values; }

    public static bool TryLookup(string name, [MaybeNullWhen(false)] out TypeSymbol type) => TypeMap.Value.TryGetValue(name, out type);
}
