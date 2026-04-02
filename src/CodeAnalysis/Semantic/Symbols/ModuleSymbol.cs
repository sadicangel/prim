using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class ModuleSymbol(SyntaxNode Syntax, string Name, ModuleSymbol ContainingModule)
    : ContainerSymbol(SymbolKind.Module, Syntax, Name, ContainingModule.RuntimeType, ContainingModule, ContainingModule, Modifiers.Static | Modifiers.ReadOnly)
{
    public StructTypeSymbol RuntimeType => Get<StructTypeSymbol>("type");
    public StructTypeSymbol Any => Get<StructTypeSymbol>("any");
    public StructTypeSymbol Unknown => Get<StructTypeSymbol>("unknown");
    public StructTypeSymbol Never => Get<StructTypeSymbol>("never");
    public StructTypeSymbol Unit => Get<StructTypeSymbol>("unit");
    public StructTypeSymbol Str => Get<StructTypeSymbol>("str");
    public StructTypeSymbol Bool => Get<StructTypeSymbol>("bool");
    public StructTypeSymbol I8 => Get<StructTypeSymbol>("i8");
    public StructTypeSymbol I16 => Get<StructTypeSymbol>("i16");
    public StructTypeSymbol I32 => Get<StructTypeSymbol>("i32");
    public StructTypeSymbol I64 => Get<StructTypeSymbol>("i64");
    public StructTypeSymbol Isz => Get<StructTypeSymbol>("isz");
    public StructTypeSymbol U8 => Get<StructTypeSymbol>("u8");
    public StructTypeSymbol U16 => Get<StructTypeSymbol>("u16");
    public StructTypeSymbol U32 => Get<StructTypeSymbol>("u32");
    public StructTypeSymbol U64 => Get<StructTypeSymbol>("u64");
    public StructTypeSymbol Usz => Get<StructTypeSymbol>("usz");
    public StructTypeSymbol F16 => Get<StructTypeSymbol>("f16");
    public StructTypeSymbol F32 => Get<StructTypeSymbol>("f32");
    public StructTypeSymbol F64 => Get<StructTypeSymbol>("f64");

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

    public override string FullName => field ??= Name is "<global>" ? Name : $"{ContainingModule.FullName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";

    public bool Equals(ModuleSymbol? other) => other is not null && SymbolKind == other.SymbolKind && FullName == other.FullName;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, FullName);
}
