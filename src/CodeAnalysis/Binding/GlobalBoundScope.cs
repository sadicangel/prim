using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;

internal sealed class GlobalBoundScope : BoundScope
{
    private static GlobalBoundScope? s_instance;
    public static GlobalBoundScope Instance
    {
        get
        {
            if (s_instance is null)
                Interlocked.CompareExchange(ref s_instance, new GlobalBoundScope(), null);
            return s_instance;
        }
    }

    private GlobalBoundScope() : base()
    {
        Symbols = Predefined.All().ToDictionary(s => s.Name, s => s);
    }

    public TypeSymbol Any { get => (TypeSymbol)Symbols![Predefined.Any.Name]; }
    public TypeSymbol Err { get => (TypeSymbol)Symbols![Predefined.Err.Name]; }
    public TypeSymbol Unknown { get => (TypeSymbol)Symbols![Predefined.Unknown.Name]; }
    public TypeSymbol Never { get => (TypeSymbol)Symbols![Predefined.Never.Name]; }
    public TypeSymbol Unit { get => (TypeSymbol)Symbols![Predefined.Unit.Name]; }
    public TypeSymbol Type { get => (TypeSymbol)Symbols![Predefined.Type.Name]; }
    public TypeSymbol Str { get => (TypeSymbol)Symbols![Predefined.Str.Name]; }
    public TypeSymbol Bool { get => (TypeSymbol)Symbols![Predefined.Bool.Name]; }
    public TypeSymbol I8 { get => (TypeSymbol)Symbols![Predefined.I8.Name]; }
    public TypeSymbol I16 { get => (TypeSymbol)Symbols![Predefined.I16.Name]; }
    public TypeSymbol I32 { get => (TypeSymbol)Symbols![Predefined.I32.Name]; }
    public TypeSymbol I64 { get => (TypeSymbol)Symbols![Predefined.I64.Name]; }
    public TypeSymbol I128 { get => (TypeSymbol)Symbols![Predefined.I128.Name]; }
    public TypeSymbol ISize { get => (TypeSymbol)Symbols![Predefined.ISize.Name]; }
    public TypeSymbol U8 { get => (TypeSymbol)Symbols![Predefined.U8.Name]; }
    public TypeSymbol U16 { get => (TypeSymbol)Symbols![Predefined.U16.Name]; }
    public TypeSymbol U32 { get => (TypeSymbol)Symbols![Predefined.U32.Name]; }
    public TypeSymbol U64 { get => (TypeSymbol)Symbols![Predefined.U64.Name]; }
    public TypeSymbol U128 { get => (TypeSymbol)Symbols![Predefined.U128.Name]; }
    public TypeSymbol USize { get => (TypeSymbol)Symbols![Predefined.USize.Name]; }
    public TypeSymbol F16 { get => (TypeSymbol)Symbols![Predefined.F16.Name]; }
    public TypeSymbol F32 { get => (TypeSymbol)Symbols![Predefined.F32.Name]; }
    public TypeSymbol F64 { get => (TypeSymbol)Symbols![Predefined.F64.Name]; }
    public TypeSymbol F80 { get => (TypeSymbol)Symbols![Predefined.F80.Name]; }
    public TypeSymbol F128 { get => (TypeSymbol)Symbols![Predefined.F128.Name]; }
}
