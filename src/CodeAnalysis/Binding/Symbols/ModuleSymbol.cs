using System.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ModuleSymbol(
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    ModuleSymbol ContainingModule)
    : Symbol(
        BoundKind.ModuleSymbol,
        Syntax,
        Name,
        Type,
        ContainingModule,
        IsStatic: true,
        IsReadOnly: true),
    IBoundScope
{
    private readonly Dictionary<string, Symbol> _symbols = [];

    public bool IsGlobalModule => Name == Global.Name;

    public IBoundScope Parent => ContainingModule;

    public ModuleSymbol Module => this;

    public ModuleSymbol Global => FindGlobalModule(this);

    public StructTypeSymbol RuntimeType => Module.Lookup("type") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'type'");
    public StructTypeSymbol ModuleType => Module.Lookup("module") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'module'");
    public StructTypeSymbol Any => Module.Lookup("any") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'any'");
    public StructTypeSymbol Err => Module.Lookup("err") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'err'");
    public StructTypeSymbol Unknown => Module.Lookup("unknown") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'unknown'");
    public StructTypeSymbol Never => Module.Lookup("never") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'never'");
    public StructTypeSymbol Unit => Module.Lookup("unit") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'unit'");
    public StructTypeSymbol Str => Module.Lookup("str") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'str'");
    public StructTypeSymbol Bool => Module.Lookup("bool") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'bool'");
    public StructTypeSymbol I8 => Module.Lookup("i8") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i8'");
    public StructTypeSymbol I16 => Module.Lookup("i16") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i16'");
    public StructTypeSymbol I32 => Module.Lookup("i32") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i32'");
    public StructTypeSymbol I64 => Module.Lookup("i64") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i64'");
    public StructTypeSymbol I128 => Module.Lookup("i128") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i128'");
    public StructTypeSymbol Isz => Module.Lookup("isz") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'isz'");
    public StructTypeSymbol U8 => Module.Lookup("u8") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u8'");
    public StructTypeSymbol U16 => Module.Lookup("u16") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u16'");
    public StructTypeSymbol U32 => Module.Lookup("u32") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u32'");
    public StructTypeSymbol U64 => Module.Lookup("u64") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u64'");
    public StructTypeSymbol U128 => Module.Lookup("u128") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u128'");
    public StructTypeSymbol Usz => Module.Lookup("usz") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'usz'");
    public StructTypeSymbol F16 => Module.Lookup("f16") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f16'");
    public StructTypeSymbol F32 => Module.Lookup("f32") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f32'");
    public StructTypeSymbol F64 => Module.Lookup("f64") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f64'");
    public StructTypeSymbol F80 => Module.Lookup("f80") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f80'");
    public StructTypeSymbol F128 => Module.Lookup("f128") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f128'");

    private static ModuleSymbol FindGlobalModule(ModuleSymbol module)
    {
        while (module != module.ContainingModule)
            module = module.ContainingModule;
        return module;
    }

    public override IEnumerable<Symbol> DeclaredSymbols => _symbols.Values;
    public bool Declare(Symbol symbol) => _symbols.TryAdd(symbol.Name, symbol);

    public Symbol? Lookup(string name)
    {
        if (_symbols.TryGetValue(name, out var symbol))
        {
            return symbol;
        }

        if (!ReferenceEquals(this, Parent))
        {
            return Parent.Lookup(name);
        }

        return null;
    }

    public bool Replace(Symbol symbol)
    {
        if (_symbols.ContainsKey(symbol.Name))
        {
            _symbols[symbol.Name] = symbol;
            return true;
        }

        if (!ReferenceEquals(this, Parent))
        {
            return Parent.Replace(symbol);
        }

        return false;
    }

    public bool DeclareModule(string name, out ModuleSymbol moduleSymbol) =>
        DeclareModule(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, out moduleSymbol);
    public bool DeclareModule(SyntaxNode syntax, string name, out ModuleSymbol moduleSymbol)
        => Declare(moduleSymbol = new ModuleSymbol(syntax, name, ModuleType, Module));

    public bool DeclareStruct(string name, out StructTypeSymbol structTypeSymbol)
        => Declare(structTypeSymbol = CreateStructType(name));
    public bool DeclareStruct(SyntaxNode syntax, string name, out StructTypeSymbol structTypeSymbol)
        => Declare(structTypeSymbol = CreateStructType(name, syntax));

    public bool DeclareVariable(string name, TypeSymbol type, bool isStatic, bool isReadOnly, out VariableSymbol variableSymbol)
        => DeclareVariable(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, type, isStatic, isReadOnly, out variableSymbol);
    public bool DeclareVariable(SyntaxNode syntax, string name, TypeSymbol type, bool isStatic, bool isReadOnly, out VariableSymbol variableSymbol)
        => Declare(variableSymbol = new VariableSymbol(syntax, name, type, Module, isStatic, isReadOnly));

    public ArrayTypeSymbol CreateArrayType(TypeSymbol elementType, int length, SyntaxNode? syntax = null) =>
        new(syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.ArrayType), elementType, length, I32, RuntimeType, Module);

    public ErrorTypeSymbol CreateErrorType(TypeSymbol valueType, SyntaxNode? syntax = null) =>
        new(syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.ErrorType), Err, valueType, RuntimeType, Module);

    public LambdaTypeSymbol CreateLambdaType(IEnumerable<Parameter> parameters, TypeSymbol returnType, SyntaxNode? syntax = null) =>
        new(syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.LambdaType), parameters, returnType, RuntimeType, Module);

    public OptionTypeSymbol CreateOptionType(TypeSymbol underlyingType, SyntaxNode? syntax = null) =>
        new(syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.OptionType), underlyingType, RuntimeType, Module);

    public PointerTypeSymbol CreatePointerType(TypeSymbol elementType, SyntaxNode? syntax = null) =>
        new(syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.PointerType), elementType, RuntimeType, Module);

    public StructTypeSymbol CreateStructType(string name, SyntaxNode? syntax = null) =>
        new(syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, RuntimeType, Module);

    public UnionTypeSymbol CreateUnionType(IEnumerable<TypeSymbol> types, SyntaxNode? syntax = null) =>
        new(syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.UnionType), types, RuntimeType, Module);
}
