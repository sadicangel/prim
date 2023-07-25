using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeAnalysis.Symbols;

internal static class BuiltinFunctions
{
    public static readonly FunctionSymbol Print = new("print", PredefinedTypes.Void, new ParameterSymbol("value", PredefinedTypes.Any));
    public static readonly FunctionSymbol Scan = new("scan", PredefinedTypes.Str, Array.Empty<ParameterSymbol>());
    public static readonly FunctionSymbol ToStr = new("toStr", PredefinedTypes.Str, new ParameterSymbol("value", PredefinedTypes.Any));
    public static readonly FunctionSymbol IsSame = new("isSame", PredefinedTypes.Bool, new ParameterSymbol("left", PredefinedTypes.Any), new ParameterSymbol("right", PredefinedTypes.Any));
    public static readonly FunctionSymbol Random = new("random", PredefinedTypes.I32, new ParameterSymbol("max", PredefinedTypes.I32));
    public static readonly FunctionSymbol TypeOf = new("typeof", PredefinedTypes.Type, new ParameterSymbol("obj", PredefinedTypes.Any));
    public static readonly FunctionSymbol CrlType = new("clrType", PredefinedTypes.Str, new ParameterSymbol("obj", PredefinedTypes.Any));

    private static readonly Lazy<ConcurrentDictionary<string, FunctionSymbol>> FunctionMap = new(() => new ConcurrentDictionary<string, FunctionSymbol>(typeof(BuiltinFunctions)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(FunctionSymbol))
        .Select(f => (FunctionSymbol)f.GetValue(null)!)
        .ToDictionary(f => f.Name)));

    public static IEnumerable<FunctionSymbol> All { get => FunctionMap.Value.Values; }

    public static bool TryLookup(string name, [MaybeNullWhen(false)] out FunctionSymbol function) => FunctionMap.Value.TryGetValue(name, out function);
}