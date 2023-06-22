using System.Collections.Concurrent;
using System.Reflection;

namespace CodeAnalysis.Symbols;

public sealed record class TypeSymbol(string Name) : Symbol(Name, SymbolKind.Type)
{
    private static readonly Lazy<ConcurrentDictionary<string, TypeSymbol>> TypeMap = new(() => new ConcurrentDictionary<string, TypeSymbol>(typeof(TypeSymbol)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(TypeSymbol))
        .Select(f => (TypeSymbol)f.GetValue(null)!)
        .ToDictionary(f => f.Name)));

    public static readonly TypeSymbol Never = new("never");

    public static readonly TypeSymbol Any = new("any");

    public static readonly TypeSymbol Void = new("void");

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

    public static readonly TypeSymbol String = new("str");

    public static TypeSymbol? GetTypeSymbol(string name) => TypeMap.Value.GetValueOrDefault(name);

    public bool Equals(TypeSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => base.ToString();

    public bool IsAssignableFrom(TypeSymbol from) => CanAssign(from, this);
    public bool IsAssignableTo(TypeSymbol to) => CanAssign(this, to);

    private static bool CanAssign(TypeSymbol from, TypeSymbol to) => from is not null && to is not null && (to == Any || to == from);

    public Type GetClrType()
    {
        return Name switch
        {
            "never" => typeof(void),
            "any" => typeof(object),
            "void" => typeof(void),
            "bool" => typeof(bool),
            "i8" => typeof(sbyte),
            "i16" => typeof(short),
            "i32" => typeof(int),
            "i64" => typeof(long),
            "u8" => typeof(byte),
            "u16" => typeof(ushort),
            "u32" => typeof(uint),
            "u64" => typeof(ulong),
            "f32" => typeof(float),
            "f64" => typeof(double),
            "str" => typeof(string),
            _ => throw new NotImplementedException(Name)
        };
    }

    internal object? Convert(object? value)
    {
        return Name switch
        {
            "never" => null,
            "any" => value,
            "void" => null,
            "bool" => System.Convert.ToBoolean(value),
            "i8" => System.Convert.ToSByte(value),
            "i16" => System.Convert.ToInt16(value),
            "i32" => System.Convert.ToInt32(value),
            "i64" => System.Convert.ToInt64(value),
            "u8" => System.Convert.ToByte(value),
            "u16" => System.Convert.ToUInt16(value),
            "u32" => System.Convert.ToUInt32(value),
            "u64" => System.Convert.ToUInt64(value),
            "f32" => System.Convert.ToSingle(value),
            "f64" => System.Convert.ToDouble(value),
            "str" => System.Convert.ToString(value),
            _ => throw new NotImplementedException(Name)
        };
    }

    public static TypeSymbol TypeOf(object? value)
    {
        return value switch
        {
            null => Void,

            bool => Bool,

            sbyte => I8,
            short => I16,
            int => I32,
            long => I64,

            byte => U8,
            ushort => U16,
            uint => U32,
            ulong => U32,

            float => F32,
            double => F64,

            string => String,

            _ => throw new InvalidOperationException($"Unexpected literal of type {value?.GetType()}"),
        };
    }
}