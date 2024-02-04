using static CodeAnalysis.Types.PredefinedTypeNames;

namespace CodeAnalysis.Types;
public sealed record class PredefinedType(string Name)
    : PrimType(Name)
{
    public override bool IsAssignableFrom(PrimType source)
    {
        if (Name == Never) return false;

        if (this == source) return true;

        // TODO: Consider using implicit conversion operator instead?
        return Name switch
        {
            Any => true,

            I16 => source.Name is I8 or U8,
            I32 => source.Name is I8 or U8 or I16 or U16,
            I64 => source.Name is I8 or U8 or I16 or U16 or I32 or U32,
            I128 => source.Name is I8 or U8 or I16 or U16 or I32 or U32 or I64 or U64,
            ISize => nint.Size switch
            {
                4 => source.Name is I8 or U8 or I16 or U16 or I32,
                8 => source.Name is I8 or U8 or I16 or U16 or I32 or U32 or I64,
                _ => throw new NotSupportedException($"Native size of {nint.Size}")
            },

            U16 => source.Name is U8,
            U32 => source.Name is U8 or U16,
            U64 => source.Name is U8 or U16 or U32,
            U128 => source.Name is U8 or U16 or U32 or U64,
            USize => nuint.Size switch
            {
                4 => source.Name is U8 or U16 or U32,
                8 => source.Name is U8 or U16 or U32 or U64,
                _ => throw new NotSupportedException($"Native size of {nint.Size}")
            },

            F16 => source.Name is I8 or U8,
            F32 => source.Name is I8 or U8 or I16 or U16 or F16 or I32 or U32,
            F64 => source.Name is I8 or U8 or I16 or U16 or F16 or I32 or U32 or F32 or I64 or U64,
            F80 => source.Name is I8 or U8 or I16 or U16 or F16 or I32 or U32 or F32 or I64 or U64 or F64,
            F128 => source.Name is I8 or U8 or I16 or U16 or F16 or I32 or U32 or F32 or I64 or U64 or F64 or I128 or U128 or F80,

            _ => false
        };
    }
}
