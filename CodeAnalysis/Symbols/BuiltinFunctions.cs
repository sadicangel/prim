﻿using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeAnalysis.Symbols;

internal static class BuiltinFunctions
{
    public static readonly FunctionSymbol Print = new("print", BuiltinTypes.Void, new ParameterSymbol("value", BuiltinTypes.Any));
    public static readonly FunctionSymbol Scan = new("scan", BuiltinTypes.Str, Array.Empty<ParameterSymbol>());
    public static readonly FunctionSymbol ToStr = new("toStr", BuiltinTypes.Str, new ParameterSymbol("value", BuiltinTypes.Any));
    public static readonly FunctionSymbol IsSame = new("isSame", BuiltinTypes.Bool, new ParameterSymbol("left", BuiltinTypes.Any), new ParameterSymbol("right", BuiltinTypes.Any));
    public static readonly FunctionSymbol Random = new("random", BuiltinTypes.I32, new ParameterSymbol("max", BuiltinTypes.I32));
    public static readonly FunctionSymbol TypeOf = new("typeof", BuiltinTypes.Type, new ParameterSymbol("obj", BuiltinTypes.Any));
    public static readonly FunctionSymbol CrlType = new("clrType", BuiltinTypes.Str, new ParameterSymbol("obj", BuiltinTypes.Any));

    private static readonly Lazy<ConcurrentDictionary<string, FunctionSymbol>> FunctionMap = new(() => new ConcurrentDictionary<string, FunctionSymbol>(typeof(BuiltinFunctions)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(FunctionSymbol))
        .Select(f => (FunctionSymbol)f.GetValue(null)!)
        .ToDictionary(f => f.Name)));

    public static IEnumerable<FunctionSymbol> All { get => FunctionMap.Value.Values; }

    public static bool TryLookup(string name, [MaybeNullWhen(false)] out FunctionSymbol function) => FunctionMap.Value.TryGetValue(name, out function);
}