namespace CodeAnalysis.Semantic;

internal interface IScope
{
    IScope? Parent { get; }
    ModuleSymbol Module { get; }
    bool Declare(Symbol symbol);
    Symbol? Lookup(NameString name);
    T? Lookup<T>(NameString name) where T : Symbol => Lookup(name) as T;
    Symbol? LookupLexical(NameString name);
    T? LookupLexical<T>(NameString name) where T : Symbol => LookupLexical(name) as T;
}

internal sealed class ModuleScope(ModuleSymbol module) : IScope
{
    public IScope? Parent => null;

    public ModuleSymbol Module => module;

    public bool Declare(Symbol symbol) => module.Declare(symbol);

    public Symbol? Lookup(NameString name)
    {
        if (name.IsEmpty) return null;
        if (!name.IsSimple) return module.Global.Lookup(name);

        for (var current = module; ; current = current.ContainingModule)
        {
            var symbol = current.Lookup(name);
            if (symbol is not null) return symbol;
            if (current == current.ContainingModule) return null;
        }
    }

    public Symbol? LookupLexical(NameString name) => null;
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

    public Symbol? Lookup(NameString name)
    {
        if (name.IsSimple && _locals?.GetValueOrDefault(name) is { } local)
            return local;
        return parent.Lookup(name);
    }

    public Symbol? LookupLexical(NameString name)
    {
        if (name.IsSimple && _locals?.GetValueOrDefault(name) is { } local)
            return local;
        return parent.LookupLexical(name);
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

    public Symbol? Lookup(NameString name)
    {
        if (name.IsSimple && _locals?.GetValueOrDefault(name) is { } local)
            return local;
        return Parent.Lookup(name);
    }

    public Symbol? LookupLexical(NameString name)
    {
        if (name.IsSimple && _locals?.GetValueOrDefault(name) is { } local)
            return local;
        return Parent.LookupLexical(name);
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

    public Symbol? Lookup(NameString name)
    {
        if (!name.IsSimple) return parent.Lookup(name);

        var parameter = _parameters?.GetValueOrDefault(name);
        if (parameter is not null) return parameter;

        var capture = _captures?.GetValueOrDefault(name);
        if (capture is not null) return capture;

        capture = parent.Lookup(name);
        if (capture is null) return null;

        return (_captures ??= [])[capture.Name] = capture;
    }

    public Symbol? LookupLexical(NameString name)
    {
        if (!name.IsSimple) return parent.LookupLexical(name);

        var parameter = _parameters?.GetValueOrDefault(name);
        if (parameter is not null) return parameter;

        var capture = _captures?.GetValueOrDefault(name);
        if (capture is not null) return capture;

        capture = parent.LookupLexical(name);
        if (capture is null) return null;

        return (_captures ??= [])[capture.Name] = capture;
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

    public Symbol? Lookup(NameString name)
    {
        if (name.IsSimple && type.Lookup(name) is { } member)
            return member;
        return Parent.Lookup(name);
    }

    public Symbol? LookupLexical(NameString name)
    {
        if (name.IsSimple && type.Lookup(name) is { } member)
            return member;
        return Parent.LookupLexical(name);
    }
}

internal sealed class InstanceScope(IScope parent, TypeSymbol type) : IScope
{
    public IScope Parent => parent;

    public ModuleSymbol Module => parent.Module;

    public bool Declare(Symbol symbol) => false;

    public Symbol? Lookup(NameString name)
    {
        if (name.IsSimple && type.Lookup<MemberSymbol>(name) is { } member)
            return member;
        return Parent.Lookup(name);
    }

    public Symbol? LookupLexical(NameString name)
    {
        if (name.IsSimple && type.Lookup<MemberSymbol>(name) is { } member)
            return member;
        return Parent.LookupLexical(name);
    }
}