using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class ModuleSymbol(SyntaxNode Syntax, string Name, ModuleSymbol ContainingModule)
    : ContainerSymbol(SymbolKind.Module, Syntax, Name, ContainingModule.RuntimeType, ContainingModule, ContainingModule, Modifiers.Static | Modifiers.ReadOnly)
{
    public StructSymbol RuntimeType => Get<StructSymbol>("type");
    public StructSymbol Any => Get<StructSymbol>("any");
    public StructSymbol Unknown => Get<StructSymbol>("unknown");
    public StructSymbol Never => Get<StructSymbol>("never");
    public StructSymbol Unit => Get<StructSymbol>("unit");
    public StructSymbol Str => Get<StructSymbol>("str");
    public StructSymbol Bool => Get<StructSymbol>("bool");
    public StructSymbol I8 => Get<StructSymbol>("i8");
    public StructSymbol I16 => Get<StructSymbol>("i16");
    public StructSymbol I32 => Get<StructSymbol>("i32");
    public StructSymbol I64 => Get<StructSymbol>("i64");
    public StructSymbol Isz => Get<StructSymbol>("isz");
    public StructSymbol U8 => Get<StructSymbol>("u8");
    public StructSymbol U16 => Get<StructSymbol>("u16");
    public StructSymbol U32 => Get<StructSymbol>("u32");
    public StructSymbol U64 => Get<StructSymbol>("u64");
    public StructSymbol Usz => Get<StructSymbol>("usz");
    public StructSymbol F16 => Get<StructSymbol>("f16");
    public StructSymbol F32 => Get<StructSymbol>("f32");
    public StructSymbol F64 => Get<StructSymbol>("f64");

    internal bool IsGlobal => Name == "<global>";

    private TSymbol Get<TSymbol>(string name) where TSymbol : Symbol
    {
        var current = this;
        while (true)
        {
            if (current.TryLookup<TSymbol>(name, out var symbol))
                return symbol;
            if (current == current.ContainingModule)
                throw new InvalidOperationException($"Missing {nameof(Symbol)} '{name}'");
            current = current.ContainingModule;
        }
    }
}
