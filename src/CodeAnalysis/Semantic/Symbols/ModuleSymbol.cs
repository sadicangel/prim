using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class ModuleSymbol(SyntaxNode Syntax, string Name, ModuleSymbol ContainingModule)
    : Symbol(BoundKind.ModuleSymbol, Syntax, Name, RuntimeType, ContainingModule, ContainingModule, Modifiers.Static | Modifiers.ReadOnly)
{
    public static ModuleSymbol Global
    {
        get => field ??= Factory.CreateGlobalModule();
    }

    public static TypeSymbol RuntimeType => (TypeSymbol)Global.Symbols["type"];
    public static TypeSymbol Any => (TypeSymbol)Global.Symbols["any"];
    public static TypeSymbol Unknown => (TypeSymbol)Global.Symbols["unknown"];
    public static TypeSymbol Never => (TypeSymbol)Global.Symbols["never"];
    public static TypeSymbol Unit => (TypeSymbol)Global.Symbols["unit"];
    public static TypeSymbol Str => (TypeSymbol)Global.Symbols["str"];
    public static TypeSymbol Bool => (TypeSymbol)Global.Symbols["bool"];
    public static TypeSymbol I8 => (TypeSymbol)Global.Symbols["i8"];
    public static TypeSymbol I16 => (TypeSymbol)Global.Symbols["i16"];
    public static TypeSymbol I32 => (TypeSymbol)Global.Symbols["i32"];
    public static TypeSymbol I64 => (TypeSymbol)Global.Symbols["i64"];
    public static TypeSymbol Isz => (TypeSymbol)Global.Symbols["isz"];
    public static TypeSymbol U8 => (TypeSymbol)Global.Symbols["u8"];
    public static TypeSymbol U16 => (TypeSymbol)Global.Symbols["u16"];
    public static TypeSymbol U32 => (TypeSymbol)Global.Symbols["u32"];
    public static TypeSymbol U64 => (TypeSymbol)Global.Symbols["u64"];
    public static TypeSymbol Usz => (TypeSymbol)Global.Symbols["usz"];
    public static TypeSymbol F16 => (TypeSymbol)Global.Symbols["f16"];
    public static TypeSymbol F32 => (TypeSymbol)Global.Symbols["f32"];
    public static TypeSymbol F64 => (TypeSymbol)Global.Symbols["f64"];


    internal Dictionary<string, Symbol> Symbols { get; } = [];

    public override IEnumerable<Symbol> Children() => Symbols.Values;
}

file static class Factory
{
    public static ModuleSymbol CreateGlobalModule()
    {
        var global = MakeGlobalModule();

        var type = (TypeSymbol)(global.Symbols["type"] = MakeRuntimeType(global));
        var any = (TypeSymbol)(global.Symbols["any"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.AnyKeyword), "any", global));
        var unknown = (TypeSymbol)(global.Symbols["unknown"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.UnknownKeyword), "unknown", global));
        var never = (TypeSymbol)(global.Symbols["never"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.NeverKeyword), "never", global));
        var unit = (TypeSymbol)(global.Symbols["unit"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.UnitKeyword), "unit", global));
        var str = (TypeSymbol)(global.Symbols["str"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.StrKeyword), "str", global));
        var @bool = (TypeSymbol)(global.Symbols["bool"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.BoolKeyword), "@bool", global));
        var i8 = (TypeSymbol)(global.Symbols["i8"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.I8Keyword), "i8", global));
        var i16 = (TypeSymbol)(global.Symbols["i16"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.I16Keyword), "i16", global));
        var i32 = (TypeSymbol)(global.Symbols["i32"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.I32Keyword), "i32", global));
        var i64 = (TypeSymbol)(global.Symbols["i64"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.I64Keyword), "i64", global));
        var isz = (TypeSymbol)(global.Symbols["isz"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.IszKeyword), "isz", global));
        var u8 = (TypeSymbol)(global.Symbols["u8"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.U8Keyword), "u8", global));
        var u16 = (TypeSymbol)(global.Symbols["u16"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.U16Keyword), "u16", global));
        var u32 = (TypeSymbol)(global.Symbols["u32"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.U32Keyword), "u32", global));
        var u64 = (TypeSymbol)(global.Symbols["u64"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.U64Keyword), "u64", global));
        var usz = (TypeSymbol)(global.Symbols["usz"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.UszKeyword), "usz", global));
        var f16 = (TypeSymbol)(global.Symbols["f16"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.F16Keyword), "f16", global));
        var f32 = (TypeSymbol)(global.Symbols["f32"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.F32Keyword), "f32", global));
        var f64 = (TypeSymbol)(global.Symbols["f64"] = new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.F64Keyword), "f64", global));

        // Type is it's own type. We need to avoid recursion.
        SetType(type, type);

        // Global module type `module` is contained within the global module. We need to avoid recursion.
        SetType(global, never);
        // All predefined symbols exist within the global module.

        return global;
    }


    private static ModuleSymbol MakeGlobalModule()
    {
        var module = (ModuleSymbol)RuntimeHelpers.GetUninitializedObject(typeof(ModuleSymbol)) with
        {
            BoundKind = BoundKind.ModuleSymbol,
            Syntax = SyntaxToken.CreateSynthetic(SyntaxKind.ModuleKeyword),
            Name = "<global>",
            Type = null!,
            ContainingSymbol = null!,
            ContainingModule = null!,
            Modifiers = Modifiers.Static | Modifiers.ReadOnly,
        };

        SetSymbolsProperty(module, []);
        SetContainingSymbol(module, module);
        SetContainingModule(module, module);

        return module;

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(ModuleSymbol.Symbols)}")]
        extern static void SetSymbolsProperty(ModuleSymbol module, Dictionary<string, Symbol> symbols);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(ModuleSymbol.ContainingSymbol)}")]
        extern static void SetContainingSymbol(ModuleSymbol module, Symbol containingSymbol);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(ModuleSymbol.ContainingModule)}")]
        extern static void SetContainingModule(ModuleSymbol module, ModuleSymbol containingModule);

    }

    private static StructTypeSymbol MakeRuntimeType(ModuleSymbol global)
    {
        var type = (StructTypeSymbol)RuntimeHelpers.GetUninitializedObject(typeof(StructTypeSymbol)) with
        {
            BoundKind = BoundKind.StructTypeSymbol,
            Syntax = SyntaxToken.CreateSynthetic(SyntaxKind.TypeKeyword),
            Name = "type",
            Type = null!,
            ContainingSymbol = global,
            ContainingModule = global,
            Modifiers = Modifiers.Static | Modifiers.ReadOnly,
        };

        ref var members = ref GetMembersFieldRef(type);
        members = [];

        return type;

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_members")]
        extern static ref List<Symbol> GetMembersFieldRef(TypeSymbol type);
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(TypeSymbol.Type)}")]
    private static extern void SetType(Symbol symbol, TypeSymbol type);
}
