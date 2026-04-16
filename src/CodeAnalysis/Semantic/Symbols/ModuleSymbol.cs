using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class ModuleSymbol(SyntaxNode Syntax, string Name, ModuleSymbol ContainingModule)
    : ContainerSymbol(SymbolKind.Module, Syntax, Name, ContainingModule.RuntimeType, ContainingModule, ContainingModule, Modifiers.Static | Modifiers.ReadOnly)
{
    public StructTypeSymbol RuntimeType => Get<StructTypeSymbol>(PredefinedTypeNames.Type);
    public StructTypeSymbol Any => Get<StructTypeSymbol>(PredefinedTypeNames.Any);
    public StructTypeSymbol Unknown => Get<StructTypeSymbol>(PredefinedTypeNames.Unknown);
    public StructTypeSymbol Never => Get<StructTypeSymbol>(PredefinedTypeNames.Never);
    public StructTypeSymbol Unit => Get<StructTypeSymbol>(PredefinedTypeNames.Unit);
    public StructTypeSymbol Str => Get<StructTypeSymbol>(PredefinedTypeNames.Str);
    public StructTypeSymbol Bool => Get<StructTypeSymbol>(PredefinedTypeNames.Bool);
    public StructTypeSymbol I8 => Get<StructTypeSymbol>(PredefinedTypeNames.I8);
    public StructTypeSymbol I16 => Get<StructTypeSymbol>(PredefinedTypeNames.I16);
    public StructTypeSymbol I32 => Get<StructTypeSymbol>(PredefinedTypeNames.I32);
    public StructTypeSymbol I64 => Get<StructTypeSymbol>(PredefinedTypeNames.I64);
    public StructTypeSymbol Isz => Get<StructTypeSymbol>(PredefinedTypeNames.Isz);
    public StructTypeSymbol U8 => Get<StructTypeSymbol>(PredefinedTypeNames.U8);
    public StructTypeSymbol U16 => Get<StructTypeSymbol>(PredefinedTypeNames.U16);
    public StructTypeSymbol U32 => Get<StructTypeSymbol>(PredefinedTypeNames.U32);
    public StructTypeSymbol U64 => Get<StructTypeSymbol>(PredefinedTypeNames.U64);
    public StructTypeSymbol Usz => Get<StructTypeSymbol>(PredefinedTypeNames.Usz);
    public StructTypeSymbol F16 => Get<StructTypeSymbol>(PredefinedTypeNames.F16);
    public StructTypeSymbol F32 => Get<StructTypeSymbol>(PredefinedTypeNames.F32);
    public StructTypeSymbol F64 => Get<StructTypeSymbol>(PredefinedTypeNames.F64);

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

    public override string FullyQualifiedName => field ??= Name is "<global>" ? Name : $"{ContainingModule.FullyQualifiedName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";

    public bool Equals(ModuleSymbol? other) => other is not null && SymbolKind == other.SymbolKind && FullyQualifiedName == other.FullyQualifiedName;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, FullyQualifiedName);
}
