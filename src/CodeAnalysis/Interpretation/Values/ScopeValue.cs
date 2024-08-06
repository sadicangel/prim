using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;

namespace CodeAnalysis.Interpretation.Values;
internal abstract record class ScopeValue(ScopeSymbol ScopeSymbol, ModuleValue ContainingModule, ScopeValue ContainingScope)
    : PrimValue(ContainingScope.Never)
{
    private readonly Dictionary<Symbol, PrimValue> _values = [];

    public abstract ModuleValue Module { get; }

    public string Name { get => ScopeSymbol.Name; }

    public ModuleValue GlobalModule
    {
        get
        {
            var module = Module;
            while (module != module.ContainingModule)
                module = module.ContainingModule;
            return module;
        }
    }

    public StructValue RuntimeType => (StructValue)LookupLocal(ScopeSymbol.RuntimeType);
    public StructValue Any => (StructValue)LookupLocal(ScopeSymbol.Any);
    public StructValue Err => (StructValue)LookupLocal(ScopeSymbol.Err);
    public StructValue Unknown => (StructValue)LookupLocal(ScopeSymbol.Unknown);
    public StructValue Never => (StructValue)LookupLocal(ScopeSymbol.Never);
    public StructValue Unit => (StructValue)LookupLocal(ScopeSymbol.Unit);
    public StructValue Str => (StructValue)LookupLocal(ScopeSymbol.Str);
    public StructValue Bool => (StructValue)LookupLocal(ScopeSymbol.Bool);
    public StructValue I8 => (StructValue)LookupLocal(ScopeSymbol.I8);
    public StructValue I16 => (StructValue)LookupLocal(ScopeSymbol.I16);
    public StructValue I32 => (StructValue)LookupLocal(ScopeSymbol.I32);
    public StructValue I64 => (StructValue)LookupLocal(ScopeSymbol.I64);
    public StructValue Isz => (StructValue)LookupLocal(ScopeSymbol.Isz);
    public StructValue U8 => (StructValue)LookupLocal(ScopeSymbol.U8);
    public StructValue U16 => (StructValue)LookupLocal(ScopeSymbol.U16);
    public StructValue U32 => (StructValue)LookupLocal(ScopeSymbol.U32);
    public StructValue U64 => (StructValue)LookupLocal(ScopeSymbol.U64);
    public StructValue Usz => (StructValue)LookupLocal(ScopeSymbol.Usz);
    public StructValue F16 => (StructValue)LookupLocal(ScopeSymbol.F16);
    public StructValue F32 => (StructValue)LookupLocal(ScopeSymbol.F32);
    public StructValue F64 => (StructValue)LookupLocal(ScopeSymbol.F64);

    public void Declare(Symbol symbol, PrimValue value)
    {
        if (!_values.TryAdd(symbol, value))
            throw new UnreachableException(DiagnosticMessage.SymbolRedeclaration(symbol.Name));
    }

    public PrimValue LookupLocal(Symbol symbol)
    {
        if (_values.TryGetValue(symbol, out var value))
        {
            return value;
        }

        if (!ReferenceEquals(this, ContainingScope))
        {
            return ContainingScope.LookupLocal(symbol);
        }

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public PrimValue LookupGlobal(Symbol symbol) => FindModule(symbol)._values[symbol];

    public void ReplaceLocal(Symbol symbol, PrimValue value)
    {
        if (_values.ContainsKey(symbol))
        {
            _values[symbol] = value;
            return;
        }

        if (!ReferenceEquals(this, ContainingScope))
        {
            ContainingScope.ReplaceLocal(symbol, value);
        }

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public void ReplaceGlobal(Symbol symbol, PrimValue value) => FindModule(symbol)._values[symbol] = value;

    private ModuleValue FindModule(Symbol symbol)
    {
        var moduleStack = new Stack<ModuleSymbol>();
        var moduleSymbol = symbol.ContainingModule;
        while (!moduleSymbol.IsGlobal)
        {
            moduleStack.Push(moduleSymbol);
            moduleSymbol = moduleSymbol.ContainingModule;
        }

        var module = GlobalModule;

        while (moduleStack.Count > 0)
            module = (ModuleValue)module.LookupLocal(moduleStack.Pop());

        return module;
    }
}
