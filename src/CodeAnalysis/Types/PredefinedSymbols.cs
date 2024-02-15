using System.Reflection;

namespace CodeAnalysis.Types;
internal static class PredefinedSymbols
{
    public static readonly Symbol Any = FromPredefinedType(PredefinedTypes.Any);
    public static readonly Symbol Unknown = FromPredefinedType(PredefinedTypes.Unknown);
    public static readonly Symbol Never = FromPredefinedType(PredefinedTypes.Never);
    public static readonly Symbol Unit = FromPredefinedType(PredefinedTypes.Unit);
    public static readonly Symbol Type = FromPredefinedType(PredefinedTypes.Type);
    public static readonly Symbol Str = FromPredefinedType(PredefinedTypes.Str);
    public static readonly Symbol Bool = FromPredefinedType(PredefinedTypes.Bool);
    public static readonly Symbol I8 = FromPredefinedType(PredefinedTypes.I8);
    public static readonly Symbol I16 = FromPredefinedType(PredefinedTypes.I16);
    public static readonly Symbol I32 = FromPredefinedType(PredefinedTypes.I32);
    public static readonly Symbol I64 = FromPredefinedType(PredefinedTypes.I64);
    public static readonly Symbol I128 = FromPredefinedType(PredefinedTypes.I128);
    public static readonly Symbol ISize = FromPredefinedType(PredefinedTypes.ISize);
    public static readonly Symbol U8 = FromPredefinedType(PredefinedTypes.U8);
    public static readonly Symbol U16 = FromPredefinedType(PredefinedTypes.U16);
    public static readonly Symbol U32 = FromPredefinedType(PredefinedTypes.U32);
    public static readonly Symbol U64 = FromPredefinedType(PredefinedTypes.U64);
    public static readonly Symbol U128 = FromPredefinedType(PredefinedTypes.U128);
    public static readonly Symbol USize = FromPredefinedType(PredefinedTypes.USize);
    public static readonly Symbol F16 = FromPredefinedType(PredefinedTypes.F16);
    public static readonly Symbol F32 = FromPredefinedType(PredefinedTypes.F32);
    public static readonly Symbol F64 = FromPredefinedType(PredefinedTypes.F64);
    public static readonly Symbol F80 = FromPredefinedType(PredefinedTypes.F80);
    public static readonly Symbol F128 = FromPredefinedType(PredefinedTypes.F128);

    public static readonly Symbol PrintLn = new(PredefinedSymbolNames.PrintLn, new FunctionType([new Parameter("obj", PredefinedTypes.Any)], PredefinedTypes.Unit));
    public static readonly Symbol ScanLn = new(PredefinedSymbolNames.ScanLn, new FunctionType([], PredefinedTypes.Str));

    public static IReadOnlyList<Symbol> All { get; } = typeof(PredefinedSymbols)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(Symbol))
        .Select(f => (Symbol)f.GetValue(null)!)
        .ToArray();

    private static Symbol FromPredefinedType(PredefinedType type) => new(type.Name, new TypeType(type));
}

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

    public const string PrintLn = "println";
    public const string ScanLn = "scanln";

    public static IReadOnlyList<string> All { get; } = typeof(PredefinedSymbolNames)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral && f.IsInitOnly)
        .Select(f => (string)f.GetValue(null)!)
        .ToArray();
}