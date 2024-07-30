using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;

internal interface IBoundScope
{
    IBoundScope Parent { get; }
    ModuleSymbol Module { get; }
    bool Declare(Symbol symbol);
    Symbol? Lookup(string name);
    bool Replace(Symbol symbol);
}
