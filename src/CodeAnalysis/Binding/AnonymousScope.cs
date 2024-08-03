using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed class AnonymousScope(IBoundScope parent) : IBoundScope
{
    private Dictionary<string, Symbol>? _symbols;

    public IBoundScope Parent => parent;
    public ModuleSymbol Module => parent.Module;

    public StructTypeSymbol RuntimeType => Module.RuntimeType;
    public StructTypeSymbol ModuleType => Module.ModuleType;
    public StructTypeSymbol Any => Module.Any;
    public StructTypeSymbol Err => Module.Err;
    public StructTypeSymbol Unknown => Module.Unknown;
    public StructTypeSymbol Never => Module.Never;
    public StructTypeSymbol Unit => Module.Unit;
    public StructTypeSymbol Str => Module.Str;
    public StructTypeSymbol Bool => Module.Bool;
    public StructTypeSymbol I8 => Module.I8;
    public StructTypeSymbol I16 => Module.I16;
    public StructTypeSymbol I32 => Module.I32;
    public StructTypeSymbol I64 => Module.I64;
    public StructTypeSymbol I128 => Module.I128;
    public StructTypeSymbol Isz => Module.Isz;
    public StructTypeSymbol U8 => Module.U8;
    public StructTypeSymbol U16 => Module.U16;
    public StructTypeSymbol U32 => Module.U32;
    public StructTypeSymbol U64 => Module.U64;
    public StructTypeSymbol U128 => Module.U128;
    public StructTypeSymbol Usz => Module.Usz;
    public StructTypeSymbol F16 => Module.F16;
    public StructTypeSymbol F32 => Module.F32;
    public StructTypeSymbol F64 => Module.F64;
    public StructTypeSymbol F80 => Module.F80;
    public StructTypeSymbol F128 => Module.F128;

    public bool Declare(Symbol symbol) => (_symbols ??= []).TryAdd(symbol.Name, symbol);

    public Symbol? Lookup(string name)
    {
        if (_symbols?.TryGetValue(name, out var symbol) is true)
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
        if (_symbols?.ContainsKey(symbol.Name) is true)
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
