using System.Collections.Immutable;

namespace CodeAnalysis.Semantic;

internal enum LookupResultKind
{
    NotFound,
    Found,
    Ambiguous
}

internal readonly record struct LookupResult(params ImmutableArray<Symbol> Candidates)
{
    public LookupResultKind Kind => (LookupResultKind)int.Clamp(Candidates.Length, 0, 2);

    public Symbol? Symbol => Candidates is [var symbol] ? symbol : null;

    public static LookupResult NotFound => new();

    public static LookupResult Found(Symbol symbol) => new(symbol);

    public static LookupResult Ambiguous(Symbol local, Symbol global) => new(local, global);
}

internal interface IScope
{
    IScope? Parent { get; }
    ModuleSymbol Module { get; }
    bool Declare(Symbol symbol);
    LookupResult Lookup(NameString name);
}

internal static class ScopeLookup
{
    public static LookupResult LookupQualified(IScope scope, NameString name)
    {
        if (name.IsEmpty) return LookupResult.NotFound;
        if (name.IsSimple) return scope.Lookup(name);

        var local = LookupLocalQualified(scope, name);
        var global = scope.Module.Global.Lookup(name);

        if (local is null && global is null) return LookupResult.NotFound;
        if (local is null) return LookupResult.Found(global!);
        if (global is null || ReferenceEquals(local, global)) return LookupResult.Found(local);

        return LookupResult.Ambiguous(local, global);
    }

    private static Symbol? LookupLocalQualified(IScope scope, NameString name)
    {
        using var enumerator = name.GetEnumerator();
        if (!enumerator.MoveNext()) return null;

        var first = scope.Lookup(enumerator.Current);
        var current = first.Kind == LookupResultKind.Found ? first.Symbol : null;
        while (current is not null && enumerator.MoveNext())
        {
            current = current is ContainerSymbol container
                ? container.Lookup(enumerator.Current)
                : null;
        }

        return current;
    }
}

internal sealed class ModuleScope(ModuleSymbol module) : IScope
{
    public IScope? Parent => null;

    public ModuleSymbol Module => module;

    public bool Declare(Symbol symbol) => module.Declare(symbol);

    public LookupResult Lookup(NameString name)
    {
        if (name.IsEmpty) return LookupResult.NotFound;
        if (!name.IsSimple) return ScopeLookup.LookupQualified(this, name);

        for (var current = module; ; current = current.ContainingModule)
        {
            var symbol = current.Lookup(name);
            if (symbol is not null) return LookupResult.Found(symbol);
            if (current == current.ContainingModule) return LookupResult.NotFound;
        }
    }
}

internal sealed class BlockScope(IScope parent) : IScope
{
    private Dictionary<NameString, VariableSymbol>? _locals;

    public IScope Parent => parent;

    public ModuleSymbol Module => parent.Module;

    public bool Declare(Symbol symbol)
    {
        if (symbol is not VariableSymbol variable) return false;
        return (_locals ??= []).TryAdd(variable.Name, variable);
    }

    public LookupResult Lookup(NameString name)
    {
        if (!name.IsSimple) return ScopeLookup.LookupQualified(this, name);
        if (_locals?.GetValueOrDefault(name) is { } local)
            return LookupResult.Found(local);
        return parent.Lookup(name);
    }
}

internal sealed class LoopScope(IScope parent) : IScope
{
    private Dictionary<NameString, VariableSymbol>? _locals;

    public IScope Parent => parent;

    public ModuleSymbol Module => parent.Module;

    public bool Declare(Symbol symbol)
    {
        if (symbol is not VariableSymbol variable) return false;
        return (_locals ??= []).TryAdd(variable.Name, variable);
    }

    public LookupResult Lookup(NameString name)
    {
        if (!name.IsSimple) return ScopeLookup.LookupQualified(this, name);
        if (_locals?.GetValueOrDefault(name) is { } local)
            return LookupResult.Found(local);
        return Parent.Lookup(name);
    }
}

internal sealed class LambdaScope(IScope parent, LambdaTypeSymbol lambdaType) : IScope
{
    private Dictionary<NameString, VariableSymbol>? _parameters;
    private Dictionary<NameString, Symbol>? _captures;

    public IScope Parent => parent;

    public ModuleSymbol Module => parent.Module;

    public LambdaTypeSymbol LambdaType => lambdaType;

    public bool Declare(Symbol symbol)
    {
        if (symbol is not VariableSymbol variable) return false;
        return (_parameters ??= []).TryAdd(variable.Name, variable);
    }

    public LookupResult Lookup(NameString name)
    {
        if (!name.IsSimple) return ScopeLookup.LookupQualified(this, name);

        var parameter = _parameters?.GetValueOrDefault(name);
        if (parameter is not null) return LookupResult.Found(parameter);

        var capture = _captures?.GetValueOrDefault(name);
        if (capture is not null) return LookupResult.Found(capture);

        var lookup = parent.Lookup(name);
        if (lookup.Kind != LookupResultKind.Found || lookup.Symbol is null) return lookup;

        capture = lookup.Symbol;
        return LookupResult.Found((_captures ??= [])[capture.Name] = capture);
    }
}

internal sealed class TypeScope(IScope parent, TypeSymbol type) : IScope
{
    public IScope Parent => parent;

    public ModuleSymbol Module => parent.Module;

    public bool Declare(Symbol symbol)
    {
        if (symbol is not MemberSymbol) return false;
        return type.Declare(symbol);
    }

    public LookupResult Lookup(NameString name)
    {
        if (!name.IsSimple) return ScopeLookup.LookupQualified(this, name);
        if (type.Lookup(name) is { } member)
            return LookupResult.Found(member);
        return Parent.Lookup(name);
    }
}

internal sealed class InstanceScope(IScope parent, TypeSymbol type) : IScope
{
    public IScope Parent => parent;

    public ModuleSymbol Module => parent.Module;

    public bool Declare(Symbol symbol) => false;

    public LookupResult Lookup(NameString name)
    {
        if (!name.IsSimple) return ScopeLookup.LookupQualified(this, name);
        if (type.Lookup<MemberSymbol>(name) is { } member)
            return LookupResult.Found(member);
        return Parent.Lookup(name);
    }
}
