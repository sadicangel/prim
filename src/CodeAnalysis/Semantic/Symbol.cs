using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal abstract record class Symbol(
    SymbolKind Kind,
    NameString Name,
    TypeSymbol Type,
    Symbol ContainingSymbol,
    ModuleSymbol ContainingModule,
    bool IsStatic,
    bool IsReadOnly,
    SyntaxNode? Syntax)
{
    public bool IsIntrinsic { get; set; }

    public sealed override string ToString() => $"{Name}: {Type.Name}";
}

internal abstract record class ContainerSymbol(
    SymbolKind Kind,
    NameString Name,
    TypeSymbol Type,
    Symbol ContainingSymbol,
    ModuleSymbol ContainingModule,
    bool IsStatic,
    bool IsReadOnly,
    SyntaxNode? Syntax) : Symbol(Kind, Name, Type, ContainingSymbol, ContainingModule, IsStatic, IsReadOnly, Syntax)
{
    private readonly Dictionary<NameString, Symbol> _members = [];
    public IEnumerable<Symbol> Members => _members.Values;

    public bool Declare(Symbol symbol) => _members.TryAdd(symbol.Name, symbol);

    public Symbol? Lookup(NameString name)
    {
        if (name.IsEmpty) return null;
        if (name.IsSimple) return _members.GetValueOrDefault(name);

        using var enumerator = name.GetEnumerator();
        Symbol? current = this;
        while (current is not null && enumerator.MoveNext())
        {
            current = current is ContainerSymbol container
                ? container._members.GetValueOrDefault(enumerator.Current)
                : null;
        }

        return current;
    }

    public T? Lookup<T>(NameString name) where T : Symbol => Lookup(name) as T;

}

internal sealed record class ModuleSymbol(NameString Name, ModuleSymbol ContainingModule, SyntaxNode? Syntax = null)
    : ContainerSymbol(SymbolKind.Module, Name, ContainingModule.ModuleType, ContainingModule, ContainingModule, IsStatic: true, IsReadOnly: true, Syntax)
{
    public bool IsGlobal => Name == NameString.GlobalName;

    public ModuleSymbol Global => field ??= FindGlobal();

    public TypeSymbol ModuleType => Global.Get(PredefinedTypeNames.Module);
    public TypeSymbol TypeType => Global.Get(PredefinedTypeNames.Type);
    public TypeSymbol AnyType => Global.Get(PredefinedTypeNames.Any);
    public TypeSymbol ErrType => Global.Get(PredefinedTypeNames.Err);
    public TypeSymbol UnknownType => Global.Get(PredefinedTypeNames.Unknown);
    public TypeSymbol NeverType => Global.Get(PredefinedTypeNames.Never);
    public TypeSymbol UnitType => Global.Get(PredefinedTypeNames.Unit);
    public TypeSymbol StrType => Global.Get(PredefinedTypeNames.Str);
    public TypeSymbol BoolType => Global.Get(PredefinedTypeNames.Bool);
    public TypeSymbol I8Type => Global.Get(PredefinedTypeNames.I8);
    public TypeSymbol I16Type => Global.Get(PredefinedTypeNames.I16);
    public TypeSymbol I32Type => Global.Get(PredefinedTypeNames.I32);
    public TypeSymbol I64Type => Global.Get(PredefinedTypeNames.I64);
    public TypeSymbol IszType => Global.Get(PredefinedTypeNames.Isz);
    public TypeSymbol U8Type => Global.Get(PredefinedTypeNames.U8);
    public TypeSymbol U16Type => Global.Get(PredefinedTypeNames.U16);
    public TypeSymbol U32Type => Global.Get(PredefinedTypeNames.U32);
    public TypeSymbol U64Type => Global.Get(PredefinedTypeNames.U64);
    public TypeSymbol UszType => Global.Get(PredefinedTypeNames.Usz);
    public TypeSymbol F16Type => Global.Get(PredefinedTypeNames.F16);
    public TypeSymbol F32Type => Global.Get(PredefinedTypeNames.F32);
    public TypeSymbol F64Type => Global.Get(PredefinedTypeNames.F64);

    private TypeSymbol Get(string name)
    {
        var current = this;
        while (true)
        {
            var symbol = current.Lookup<TypeSymbol>(name);
            if (symbol is not null) return symbol;
            if (current == current.ContainingModule)
                throw new InvalidOperationException($"Missing {nameof(Symbol)} '{name}'");
            current = current.ContainingModule;
        }
    }

    private ModuleSymbol FindGlobal()
    {
        for (var module = this; ; module = module.ContainingModule)
        {
            if (module.IsGlobal) return module;
            if (module == module.ContainingModule) break;
        }

        throw new InvalidOperationException($"Module was defined outside of {NameString.GlobalName}");
    }

    public bool Equals(ModuleSymbol? other) => other is not null && Kind == other.Kind && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(Kind, Name);
}

internal abstract record class TypeSymbol(SymbolKind Kind, NameString Name, ModuleSymbol ContainingModule, SyntaxNode? Syntax)
    : ContainerSymbol(Kind, Name, ContainingModule.TypeType, ContainingModule, ContainingModule, IsStatic: true, IsReadOnly: true, Syntax)
{
    public bool IsAny => Name == PredefinedTypeNames.Any;
    public bool IsUnit => Name == PredefinedTypeNames.Unit;
    public bool IsNever => Name == PredefinedTypeNames.Never;
    public bool IsUnknown => Name == PredefinedTypeNames.Unknown;
}

internal sealed record class ArrayTypeSymbol(TypeSymbol ElementType, long? Length, SyntaxNode? Syntax = null)
    : TypeSymbol(SymbolKind.ArrayType, $"{ElementType.Name}[{Length}]", ElementType.ContainingModule.Global, Syntax);

internal sealed record class LambdaTypeSymbol(ImmutableArray<TypeSymbol> ParameterTypes, TypeSymbol ReturnType, SyntaxNode? Syntax = null)
    : TypeSymbol(SymbolKind.LambdaType, $"({string.Join(", ", ParameterTypes.Select(t => t.Name.ToString()))}) -> {ReturnType.Name}", ReturnType.ContainingModule.Global, Syntax);

internal sealed record class PointerTypeSymbol(TypeSymbol ElementType, SyntaxNode? Syntax = null)
    : TypeSymbol(SymbolKind.PointerType, $"{ElementType.Name}*", ElementType.ContainingModule.Global, Syntax);

internal sealed record class StructTypeSymbol(NameString Name, ModuleSymbol ContainingModule, SyntaxNode? Syntax = null)
    : TypeSymbol(SymbolKind.StructType, Name, ContainingModule, Syntax);

internal sealed record class UnionTypeSymbol(ImmutableArray<TypeSymbol> Variants, SyntaxNode? Syntax = null)
    : TypeSymbol(SymbolKind.UnionType, string.Join(" | ", Variants.Select(t => t.Name.ToString())), Variants[0].ContainingModule.Global, Syntax);

internal abstract record class MemberSymbol(
    SymbolKind Kind,
    NameString Name,
    TypeSymbol Type,
    Symbol ContainingSymbol,
    bool IsStatic,
    bool IsReadOnly,
    SyntaxNode? Syntax)
    : Symbol(Kind, Name, Type, ContainingSymbol, ContainingSymbol as ModuleSymbol ?? ContainingSymbol.ContainingModule, IsStatic, IsReadOnly, Syntax);

internal sealed record class PropertySymbol(NameString Name, TypeSymbol Type, Symbol ContainingSymbol, bool IsStatic, bool IsReadOnly, SyntaxNode? Syntax = null)
    : MemberSymbol(SymbolKind.Property, Name, Type, ContainingSymbol, IsStatic, IsReadOnly, Syntax);

internal sealed record class IndexerSymbol(NameString Name, TypeSymbol Type, ImmutableArray<TypeSymbol> ParameterTypes, Symbol ContainingSymbol, bool IsStatic, bool IsReadOnly, SyntaxNode? Syntax = null)
    : MemberSymbol(SymbolKind.Indexer, Name, Type, ContainingSymbol, IsStatic, IsReadOnly, Syntax);

internal sealed record class OperatorSymbol(NameString Name, TypeSymbol Type, ImmutableArray<TypeSymbol> OperandTypes, Symbol ContainingSymbol, bool IsStatic, bool IsReadOnly, SyntaxNode? Syntax = null)
    : MemberSymbol(SymbolKind.Operator, Name, Type, ContainingSymbol, IsStatic, IsReadOnly, Syntax);

internal sealed record class ConversionSymbol(NameString Name, TypeSymbol Type, TypeSymbol OperandType, bool IsImplicit, Symbol ContainingSymbol, SyntaxNode? Syntax = null)
    : Symbol(SymbolKind.Conversion, Name, Type, ContainingSymbol, ContainingSymbol.ContainingModule, IsStatic: true, IsReadOnly: true, Syntax)
{
    public static NameString GetName(TypeSymbol source, TypeSymbol target) => $"op_{target.Name}({source.Name})";
}

internal sealed record class VariableSymbol(
    NameString Name,
    TypeSymbol Type,
    ModuleSymbol ContainingModule,
    bool IsStatic,
    bool IsReadOnly,
    SyntaxNode? Syntax = null) : Symbol(SymbolKind.Variable, Name, Type, ContainingModule, ContainingModule, IsStatic, IsReadOnly, Syntax);
