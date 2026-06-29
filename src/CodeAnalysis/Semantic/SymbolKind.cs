namespace CodeAnalysis.Semantic;

public enum SymbolKind
{
    Module,
    Property,
    Indexer,
    Operator,
    Conversion,
    Variable,
    Label,

    ArrayType,
    LambdaType,
    PointerType,
    StructType,
    UnionType,
}
