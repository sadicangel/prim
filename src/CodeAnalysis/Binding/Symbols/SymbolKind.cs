namespace CodeAnalysis.Binding.Symbols;

internal enum SymbolKind
{
    Unknown,
    ArrayType,
    FunctionType,
    NamedType,
    OptionType,
    PredefinedType,
    UnionType,

    Function,
    Parameter,

    Struct,
    Property,

    Variable,
}
