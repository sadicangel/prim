using System.Runtime.CompilerServices;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests;
internal static class Extensions
{
    public static PrimValue Evaluate(this string sourceText)
    {
        // HACK: Because global module is a static mutable, we are having fun.
        // The global scope has to be bound every time, as a new variable.
        {
            ref var symbols = ref GetSymbolsFieldRef(Predefined.GlobalModule);
            var addedSymbols = symbols.Keys.Except(Predefined.All().Select(s => s.Name)).ToList();
            foreach (var added in addedSymbols)
                symbols.Remove(added);
        }
        var compilation = Compilation.CompileScript(new SourceText(sourceText));
        Assert.Empty(compilation.Diagnostics);
        var evaluation = Evaluation.Evaluate(compilation);
        return evaluation.Values[0];



        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_symbols")]
        extern static ref Dictionary<string, Symbol> GetSymbolsFieldRef(ModuleSymbol module);
    }
}
