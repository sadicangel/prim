namespace CodeAnalysis.Symbols;

public sealed record class TypeSymbol(string Name) : Symbol(SymbolKind.Type, Name, BuiltinTypes.Type)
{
    public required Type ClrType { get; init; }

    public bool Equals(TypeSymbol? other) => other is not null && Name == other.Name;
    public override int GetHashCode() => Name.GetHashCode();
    public override string ToString() => Name;

    public bool IsAssignableFrom(TypeSymbol from) => CanAssign(from, this);
    public bool IsAssignableTo(TypeSymbol to) => CanAssign(this, to);

    private static bool CanAssign(TypeSymbol from, TypeSymbol to) => from is not null && to is not null && (to == BuiltinTypes.Any || to == from);

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
            null => BuiltinTypes.Void,

            bool => BuiltinTypes.Bool,

            sbyte => BuiltinTypes.I8,
            short => BuiltinTypes.I16,
            int => BuiltinTypes.I32,
            long => BuiltinTypes.I64,

            byte => BuiltinTypes.U8,
            ushort => BuiltinTypes.U16,
            uint => BuiltinTypes.U32,
            ulong => BuiltinTypes.U32,

            float => BuiltinTypes.F32,
            double => BuiltinTypes.F64,

            string => BuiltinTypes.Str,

            TypeSymbol => BuiltinTypes.Type,

            FunctionSymbol => BuiltinTypes.Func,

            _ => throw new InvalidOperationException($"Unexpected literal of type {value?.GetType()}"),
        };
    }
}