using System.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal abstract record class ScopeSymbol(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    ScopeSymbol ContainingScope,
    bool IsStatic,
    bool IsReadOnly)
    : Symbol(
        BoundKind,
        Syntax,
        Name,
        Type,
        ContainingScope.Module,
        ContainingScope,
        IsStatic,
        IsReadOnly)
{
    private Dictionary<string, Symbol>? _symbols = [];

    public abstract bool IsAnonymous { get; }

    public abstract ModuleSymbol Module { get; }

    public ModuleSymbol GlobalModule
    {
        get
        {
            var module = Module;
            while (module != module.ContainingModule)
                module = module.ContainingModule;
            return module;
        }
    }

    public StructTypeSymbol RuntimeType => Module.Lookup(PredefinedTypes.Type) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'type'");
    public StructTypeSymbol Any => Module.Lookup(PredefinedTypes.Any) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'any'");
    public StructTypeSymbol Err => Module.Lookup(PredefinedTypes.Err) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'err'");
    public StructTypeSymbol Unknown => Module.Lookup(PredefinedTypes.Unknown) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'unknown'");
    public StructTypeSymbol Never => Module.Lookup(PredefinedTypes.Never) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'never'");
    public StructTypeSymbol Unit => Module.Lookup(PredefinedTypes.Unit) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'unit'");
    public StructTypeSymbol Str => Module.Lookup(PredefinedTypes.Str) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'str'");
    public StructTypeSymbol Bool => Module.Lookup(PredefinedTypes.Bool) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'bool'");
    public StructTypeSymbol I8 => Module.Lookup(PredefinedTypes.I8) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i8'");
    public StructTypeSymbol I16 => Module.Lookup(PredefinedTypes.I16) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i16'");
    public StructTypeSymbol I32 => Module.Lookup(PredefinedTypes.I32) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i32'");
    public StructTypeSymbol I64 => Module.Lookup(PredefinedTypes.I64) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i64'");
    public StructTypeSymbol Isz => Module.Lookup(PredefinedTypes.Isz) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'isz'");
    public StructTypeSymbol U8 => Module.Lookup(PredefinedTypes.U8) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u8'");
    public StructTypeSymbol U16 => Module.Lookup(PredefinedTypes.U16) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u16'");
    public StructTypeSymbol U32 => Module.Lookup(PredefinedTypes.U32) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u32'");
    public StructTypeSymbol U64 => Module.Lookup(PredefinedTypes.U64) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u64'");
    public StructTypeSymbol Usz => Module.Lookup(PredefinedTypes.Usz) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'usz'");
    public StructTypeSymbol F16 => Module.Lookup(PredefinedTypes.F16) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f16'");
    public StructTypeSymbol F32 => Module.Lookup(PredefinedTypes.F32) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f32'");
    public StructTypeSymbol F64 => Module.Lookup(PredefinedTypes.F64) as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f64'");

    public override IEnumerable<Symbol> DeclaredSymbols => _symbols?.Values as IEnumerable<Symbol> ?? [];

    public bool Declare(Symbol symbol) => (_symbols ??= []).TryAdd(symbol.Name, symbol);

    public Symbol? Lookup(NameValue name)
    {
        if (name.Count == 0)
        {
            throw new UnreachableException("Tried to lookup an invalid symbol name");
        }

        return name.Count > 1 ? SearchDown(name) : SearchUp(name);

        Symbol? SearchUp(string name)
        {
            if (_symbols?.TryGetValue(name, out var symbol) is true)
            {
                return symbol;
            }

            if (!ReferenceEquals(this, ContainingScope))
            {
                return ContainingScope.Lookup(name);
            }

            return null;
        }

        Symbol? SearchDown(string[] names)
        {
            var module = GlobalModule;
            for (var i = 0; i < names.Length - 1; ++i)
            {
                if (module.Lookup(names[i]) is not ModuleSymbol m)
                    return null;

                module = m;
            }

            return module.Lookup(names[^1]);
        }
    }

    public bool Replace(Symbol symbol)
    {
        if (_symbols?.ContainsKey(symbol.Name) is true)
        {
            _symbols[symbol.Name] = symbol;
            return true;
        }

        if (!ReferenceEquals(this, ContainingScope))
        {
            return ContainingScope.Replace(symbol);
        }

        return false;
    }

    public bool DeclareModule(string name, out ModuleSymbol moduleSymbol) =>
        DeclareModule(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, out moduleSymbol);
    public bool DeclareModule(SyntaxNode syntax, string name, out ModuleSymbol moduleSymbol)
        => Declare(moduleSymbol = new ModuleSymbol(syntax, name, Module));

    public bool DeclareStruct(string name, out StructTypeSymbol structTypeSymbol)
        => DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, out structTypeSymbol);
    public bool DeclareStruct(SyntaxNode syntax, string name, out StructTypeSymbol structTypeSymbol)
        => Declare(structTypeSymbol = new StructTypeSymbol(syntax, name, Module));

    public bool DeclareVariable(string name, TypeSymbol type, bool isStatic, bool isReadOnly, out VariableSymbol variableSymbol)
        => DeclareVariable(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, type, isStatic, isReadOnly, out variableSymbol);
    public bool DeclareVariable(SyntaxNode syntax, string name, TypeSymbol type, bool isStatic, bool isReadOnly, out VariableSymbol variableSymbol)
        => Declare(variableSymbol = new VariableSymbol(syntax, name, type, Module, isStatic, isReadOnly));
}
