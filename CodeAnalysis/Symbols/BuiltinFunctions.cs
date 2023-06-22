using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeAnalysis.Symbols;

internal static class BuiltinFunctions
{
    public static readonly FunctionSymbol Print = new("print", TypeSymbol.Void, new ParameterSymbol("value", TypeSymbol.Any));
    public static readonly FunctionSymbol Input = new("input", TypeSymbol.String, Array.Empty<ParameterSymbol>());

    private static readonly Lazy<ConcurrentDictionary<string, FunctionSymbol>> FunctionMap = new(() => new ConcurrentDictionary<string, FunctionSymbol>(typeof(BuiltinFunctions)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(FunctionSymbol))
        .Select(f => (FunctionSymbol)f.GetValue(null)!)
        .ToDictionary(f => f.Name)));

    public static IEnumerable<FunctionSymbol> All { get => FunctionMap.Value.Values; }

    public static bool TryLookup(string name, [MaybeNullWhen(false)] out FunctionSymbol function) => FunctionMap.Value.TryGetValue(name, out function);
}