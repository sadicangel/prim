using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class ModuleValue(ModuleSymbol Module, ModuleValue ContainingModule, IBoundScope BoundScope) : PrimValue(Module.Type), IEvaluatedScope
{
    private readonly Dictionary<Symbol, PrimValue> _values = [];

    public string Name { get => Module.Name; }
    public override ModuleSymbol Value => Module;

    public IEvaluatedScope Parent { get => ContainingModule ?? this; }

    public StructValue RuntimeType => (StructValue)Lookup(BoundScope.RuntimeType);
    public StructValue Any => (StructValue)Lookup(BoundScope.Any);
    public StructValue Err => (StructValue)Lookup(BoundScope.Err);
    public StructValue Unknown => (StructValue)Lookup(BoundScope.Unknown);
    public StructValue Never => (StructValue)Lookup(BoundScope.Never);
    public StructValue Unit => (StructValue)Lookup(BoundScope.Unit);
    public StructValue Str => (StructValue)Lookup(BoundScope.Str);
    public StructValue Bool => (StructValue)Lookup(BoundScope.Bool);
    public StructValue I8 => (StructValue)Lookup(BoundScope.I8);
    public StructValue I16 => (StructValue)Lookup(BoundScope.I16);
    public StructValue I32 => (StructValue)Lookup(BoundScope.I32);
    public StructValue I64 => (StructValue)Lookup(BoundScope.I64);
    public StructValue Isz => (StructValue)Lookup(BoundScope.Isz);
    public StructValue U8 => (StructValue)Lookup(BoundScope.U8);
    public StructValue U16 => (StructValue)Lookup(BoundScope.U16);
    public StructValue U32 => (StructValue)Lookup(BoundScope.U32);
    public StructValue U64 => (StructValue)Lookup(BoundScope.U64);
    public StructValue Usz => (StructValue)Lookup(BoundScope.Usz);
    public StructValue F16 => (StructValue)Lookup(BoundScope.F16);
    public StructValue F32 => (StructValue)Lookup(BoundScope.F32);
    public StructValue F64 => (StructValue)Lookup(BoundScope.F64);

    ModuleValue IEvaluatedScope.Module { get => this; }

    public void Declare(Symbol symbol, PrimValue value)
    {
        if (!_values.TryAdd(symbol, value))
            throw new UnreachableException(DiagnosticMessage.SymbolRedeclaration(symbol.Name));
    }

    public PrimValue Lookup(Symbol symbol)
    {
        if (_values.TryGetValue(symbol, out var value))
        {
            return value;
        }

        if (!ReferenceEquals(this, Parent))
        {
            return Parent.Lookup(symbol);
        }

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public void Replace(Symbol symbol, PrimValue value)
    {
        if (_values.ContainsKey(symbol))
        {
            _values[symbol] = value;
            return;
        }

        if (!ReferenceEquals(this, Parent))
        {
            Parent.Replace(symbol, value);
        }

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }
}
