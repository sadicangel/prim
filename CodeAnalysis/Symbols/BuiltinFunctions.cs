﻿using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeAnalysis.Symbols;

internal static class BuiltinFunctions
{
    public static readonly FunctionSymbol WriteLine = new("writeLine", TypeSymbol.Void, new ParameterSymbol("value", TypeSymbol.Any));
    public static readonly FunctionSymbol ReadLine = new("readLine", TypeSymbol.String, Array.Empty<ParameterSymbol>());
    public static readonly FunctionSymbol ToStr = new("toStr", TypeSymbol.String, new ParameterSymbol("value", TypeSymbol.Any));
    public static readonly FunctionSymbol IsSame = new("isSame", TypeSymbol.Bool, new ParameterSymbol("left", TypeSymbol.Any), new ParameterSymbol("right", TypeSymbol.Any));
    public static readonly FunctionSymbol Random = new("random", TypeSymbol.I32, new ParameterSymbol("max", TypeSymbol.I32));

    private static readonly Lazy<ConcurrentDictionary<string, FunctionSymbol>> FunctionMap = new(() => new ConcurrentDictionary<string, FunctionSymbol>(typeof(BuiltinFunctions)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(FunctionSymbol))
        .Select(f => (FunctionSymbol)f.GetValue(null)!)
        .ToDictionary(f => f.Name)));

    public static IEnumerable<FunctionSymbol> All { get => FunctionMap.Value.Values; }

    public static bool TryLookup(string name, [MaybeNullWhen(false)] out FunctionSymbol function) => FunctionMap.Value.TryGetValue(name, out function);
}