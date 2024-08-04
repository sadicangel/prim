using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;

internal sealed class AnonymousScope(IEvaluatedScope parent) : IEvaluatedScope
{
    private Dictionary<Symbol, PrimValue>? _values;

    public IEvaluatedScope Parent => parent;

    public ModuleValue Module => parent.Module;

    public StructValue RuntimeType => Module.RuntimeType;
    public StructValue Any => Module.Any;
    public StructValue Err => Module.Err;
    public StructValue Unknown => Module.Unknown;
    public StructValue Never => Module.Never;
    public StructValue Unit => Module.Unit;
    public StructValue Str => Module.Str;
    public StructValue Bool => Module.Bool;
    public StructValue I8 => Module.I8;
    public StructValue I16 => Module.I16;
    public StructValue I32 => Module.I32;
    public StructValue I64 => Module.I64;
    public StructValue Isz => Module.Isz;
    public StructValue U8 => Module.U8;
    public StructValue U16 => Module.U16;
    public StructValue U32 => Module.U32;
    public StructValue U64 => Module.U64;
    public StructValue Usz => Module.Usz;
    public StructValue F16 => Module.F16;
    public StructValue F32 => Module.F32;
    public StructValue F64 => Module.F64;

    public void Declare(Symbol symbol, PrimValue value)
    {
        if (!(_values ??= []).TryAdd(symbol, value))
            throw new UnreachableException(DiagnosticMessage.SymbolRedeclaration(symbol.Name));
    }

    public PrimValue Lookup(Symbol symbol)
    {
        if (_values?.TryGetValue(symbol, out var value) is true)
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
        if (_values?.ContainsKey(symbol) is true)
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

