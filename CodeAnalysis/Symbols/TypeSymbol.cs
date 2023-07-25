using System.Runtime.CompilerServices;

namespace CodeAnalysis.Symbols;

public sealed record class TypeSymbol(string Name) : Symbol(SymbolKind.Type, Name, PredefinedTypes.Type)
{
    public Type ClrType
    {
        get => Name switch
        {
            PredefinedTypeNames.Never => typeof(void),
            PredefinedTypeNames.Any => typeof(object),
            PredefinedTypeNames.Void => typeof(void),
            PredefinedTypeNames.Type => typeof(Type),
            PredefinedTypeNames.Bool => typeof(bool),
            PredefinedTypeNames.I8 => typeof(sbyte),
            PredefinedTypeNames.I16 => typeof(short),
            PredefinedTypeNames.I32 => typeof(int),
            PredefinedTypeNames.I64 => typeof(long),
            PredefinedTypeNames.ISize => typeof(nint),
            PredefinedTypeNames.U8 => typeof(byte),
            PredefinedTypeNames.U16 => typeof(ushort),
            PredefinedTypeNames.U32 => typeof(uint),
            PredefinedTypeNames.U64 => typeof(ulong),
            PredefinedTypeNames.USize => typeof(nuint),
            PredefinedTypeNames.F32 => typeof(float),
            PredefinedTypeNames.F64 => typeof(double),
            PredefinedTypeNames.Str => typeof(string),
            PredefinedTypeNames.Func => typeof(Delegate),
            _ => throw new InvalidOperationException($"CLR Type for {Name} is not defined")
        };
    }

    public int BinarySize
    {
        get => Name switch
        {
            PredefinedTypeNames.Bool => sizeof(bool),
            PredefinedTypeNames.I8 => sizeof(sbyte),
            PredefinedTypeNames.I16 => sizeof(short),
            PredefinedTypeNames.I32 => sizeof(int),
            PredefinedTypeNames.I64 => sizeof(long),
            PredefinedTypeNames.ISize => Unsafe.SizeOf<nint>(),
            PredefinedTypeNames.U8 => sizeof(byte),
            PredefinedTypeNames.U16 => sizeof(ushort),
            PredefinedTypeNames.U32 => sizeof(uint),
            PredefinedTypeNames.U64 => sizeof(ulong),
            PredefinedTypeNames.USize => Unsafe.SizeOf<nuint>(),
            PredefinedTypeNames.F32 => sizeof(float),
            PredefinedTypeNames.F64 => sizeof(double),
            _ => throw new InvalidOperationException($"Size of {Name} is not known at compile time")
        };
    }

    public bool IsBinary { get => Name is PredefinedTypeNames.Bool || IsNumber; }

    public bool IsNumber { get => IsInteger || IsFloatingPoint; }

    public bool IsInteger { get => IsSignedInteger || IsUnsignedInteger; }

    public bool IsSignedInteger { get => Name is PredefinedTypeNames.I8 or PredefinedTypeNames.I16 or PredefinedTypeNames.I32 or PredefinedTypeNames.I64 or PredefinedTypeNames.ISize; }

    public bool IsUnsignedInteger { get => Name is PredefinedTypeNames.U8 or PredefinedTypeNames.U16 or PredefinedTypeNames.U32 or PredefinedTypeNames.U64 or PredefinedTypeNames.USize; }

    public bool IsFloatingPoint { get => Name is PredefinedTypeNames.F32 or PredefinedTypeNames.F64; }

    public bool Equals(TypeSymbol? other) => other is not null && Name == other.Name;
    public override int GetHashCode() => Name.GetHashCode();
    public override string ToString() => Name;

    public bool IsAssignableFrom(TypeSymbol from) => CanAssign(from, this);
    public bool IsAssignableTo(TypeSymbol to) => CanAssign(this, to);

    private static bool CanAssign(TypeSymbol from, TypeSymbol to) => from is not null && to is not null && (to == PredefinedTypes.Any || to == from);

    internal object? Convert(object? value)
    {
        return Name switch
        {
            PredefinedTypeNames.Never => null,
            PredefinedTypeNames.Any => value,
            PredefinedTypeNames.Void => null,
            PredefinedTypeNames.Bool => System.Convert.ToBoolean(value),
            PredefinedTypeNames.I8 => System.Convert.ToSByte(value),
            PredefinedTypeNames.I16 => System.Convert.ToInt16(value),
            PredefinedTypeNames.I32 => System.Convert.ToInt32(value),
            PredefinedTypeNames.I64 => System.Convert.ToInt64(value),
            PredefinedTypeNames.ISize => IntPtr.CreateChecked(IntPtr.Size == sizeof(long) ? System.Convert.ToInt64(value) : System.Convert.ToInt32(value)),
            PredefinedTypeNames.U8 => System.Convert.ToByte(value),
            PredefinedTypeNames.U16 => System.Convert.ToUInt16(value),
            PredefinedTypeNames.U32 => System.Convert.ToUInt32(value),
            PredefinedTypeNames.U64 => System.Convert.ToUInt64(value),
            PredefinedTypeNames.USize => UIntPtr.CreateChecked(UIntPtr.Size == sizeof(long) ? System.Convert.ToUInt64(value) : System.Convert.ToUInt32(value)),
            PredefinedTypeNames.F32 => System.Convert.ToSingle(value),
            PredefinedTypeNames.F64 => System.Convert.ToDouble(value),
            PredefinedTypeNames.Str => System.Convert.ToString(value),
            _ => throw new NotImplementedException(Name)
        };
    }

    public static TypeSymbol TypeOf(object? value)
    {
        return value switch
        {
            null => PredefinedTypes.Void,

            bool => PredefinedTypes.Bool,

            sbyte => PredefinedTypes.I8,
            short => PredefinedTypes.I16,
            int => PredefinedTypes.I32,
            long => PredefinedTypes.I64,

            byte => PredefinedTypes.U8,
            ushort => PredefinedTypes.U16,
            uint => PredefinedTypes.U32,
            ulong => PredefinedTypes.U32,

            float => PredefinedTypes.F32,
            double => PredefinedTypes.F64,

            string => PredefinedTypes.Str,

            TypeSymbol => PredefinedTypes.Type,

            FunctionSymbol => PredefinedTypes.Func,

            _ => throw new InvalidOperationException($"Unexpected literal of {value?.GetType()}"),
        };
    }
}