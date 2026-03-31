using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Semantic.Symbols;

internal static class SymbolFactory
{
    public static ModuleSymbol CreateGlobalModule()
    {
        var global = MakeGlobalModule();

        var type = Declare(global, MakeRuntimeType(global));
        var never = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.NeverKeyword), "never", global));

        var any = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.AnyKeyword), "any", global));
        var unknown = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.UnknownKeyword), "unknown", global));
        var unit = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.UnitKeyword), "unit", global));
        var str = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.StrKeyword), "str", global));
        var @bool = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.BoolKeyword), "bool", global));
        var i8 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.I8Keyword), "i8", global));
        var i16 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.I16Keyword), "i16", global));
        var i32 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.I32Keyword), "i32", global));
        var i64 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.I64Keyword), "i64", global));
        var isz = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.IszKeyword), "isz", global));
        var u8 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.U8Keyword), "u8", global));
        var u16 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.U16Keyword), "u16", global));
        var u32 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.U32Keyword), "u32", global));
        var u64 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.U64Keyword), "u64", global));
        var usz = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.UszKeyword), "usz", global));
        var f16 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.F16Keyword), "f16", global));
        var f32 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.F32Keyword), "f32", global));
        var f64 = Declare(global, new StructTypeSymbol(SyntaxToken.CreateSynthetic(SyntaxKind.F64Keyword), "f64", global));

        // Type is its own type. We need to avoid recursion.
        SetType(type, type);

        // Global module type `module` is contained within the global module. We need to avoid recursion.
        SetType(global, never); // TODO: Should be something else than `never`.
        // All predefined symbols exist within the global module.


        DeclareEqualityOperators(str, @bool, i8, i16, i32, i64, isz, u8, u16, u32, u64, usz, f16, f32, f64);
        DeclareComparisonOperators(i8, i16, i32, i64, isz, u8, u16, u32, u64, usz, f16, f32, f64);
        DeclareMathOperators(i8, i16, i32, i64, isz, u8, u16, u32, u64, usz, f16, f32, f64);
        DeclareBitwiseOperators(i8, i16, i32, i64, isz, u8, u16, u32, u64, usz);
        DeclareLogicalOperators(@bool);
        DeclareStringOperators(str);

        // TODO: Missing conversions.

        return global;

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(TypeSymbol.Type)}")]
        static extern void SetType(Symbol symbol, TypeSymbol type);
    }

    private static ModuleSymbol MakeGlobalModule()
    {
        var module = (ModuleSymbol)RuntimeHelpers.GetUninitializedObject(typeof(ModuleSymbol)) with
        {
            SymbolKind = SymbolKind.Module,
            Syntax = new ModuleDeclarationSyntax(
                SyntaxToken.CreateSynthetic(SyntaxKind.ModuleKeyword),
                new SyntheticSimpleNameSyntax("<global>"),
                SyntaxToken.CreateSynthetic(SyntaxKind.SemicolonToken)),
            Name = "<global>",
            Type = null!,
            ContainingSymbol = null!,
            ContainingModule = null!,
            Modifiers = Modifiers.Static | Modifiers.ReadOnly,
        };

        GetMembersField(module) = [];
        SetContainingSymbol(module, module);
        SetContainingModule(module, module);

        return module;

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(Symbol.ContainingSymbol)}")]
        static extern void SetContainingSymbol(Symbol symbol, Symbol containingSymbol);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(Symbol.ContainingModule)}")]
        static extern void SetContainingModule(Symbol symbol, ModuleSymbol containingModule);
    }

    private static StructTypeSymbol MakeRuntimeType(ModuleSymbol global)
    {
        var type = (StructTypeSymbol)RuntimeHelpers.GetUninitializedObject(typeof(StructTypeSymbol)) with
        {
            SymbolKind = SymbolKind.Struct,
            Syntax = SyntaxToken.CreateSynthetic(SyntaxKind.TypeKeyword),
            Name = "type",
            Type = null!,
            ContainingSymbol = global,
            ContainingModule = global,
            Modifiers = Modifiers.Static | Modifiers.ReadOnly,
        };

        GetMembersField(type) = [];

        return type;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_members")]
    static extern ref Dictionary<string, Symbol> GetMembersField(ContainerSymbol container);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_IsPredefined")]
    static extern void SetIsPredefined(Symbol symbol, bool value);

    private static T Declare<T>(ContainerSymbol container, T symbol) where T : Symbol
    {
        if (!container.TryDeclare(symbol))
            throw new InvalidOperationException($"Could not declare symbol '{symbol}'");
        SetIsPredefined(symbol, true);
        return symbol;
    }

    private static void DeclareEqualityOperators(params ReadOnlySpan<TypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.EqualsEqualsToken),
                    Name: Mangler.Mangle(SyntaxKind.EqualsEqualsToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.EqualsEqualsToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.BangEqualsToken),
                    Name: Mangler.Mangle(SyntaxKind.BangEqualsToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.BangEqualsToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));
        }
    }

    private static void DeclareComparisonOperators(params ReadOnlySpan<TypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanToken),
                    Name: Mangler.Mangle(SyntaxKind.LessThanToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanEqualsToken),
                    Name: Mangler.Mangle(SyntaxKind.LessThanEqualsToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanEqualsToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanToken),
                    Name: Mangler.Mangle(SyntaxKind.GreaterThanToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanEqualsToken),
                    Name: Mangler.Mangle(SyntaxKind.GreaterThanEqualsToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanEqualsToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));
        }
    }

    private static void DeclareMathOperators(params ReadOnlySpan<TypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    Name: Mangler.Mangle(SyntaxKind.PlusToken, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                        Parameters: [type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.MinusToken),
                    Name: Mangler.Mangle(SyntaxKind.MinusToken, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.MinusToken),
                        Parameters: [type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    Name: Mangler.Mangle(SyntaxKind.PlusToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.MinusToken),
                    Name: Mangler.Mangle(SyntaxKind.MinusToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.MinusToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.StarToken),
                    Name: Mangler.Mangle(SyntaxKind.StarToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.StarToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.SlashToken),
                    Name: Mangler.Mangle(SyntaxKind.SlashToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.SlashToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PercentToken),
                    Name: Mangler.Mangle(SyntaxKind.PercentToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PercentToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.StarStarToken),
                    Name: Mangler.Mangle(SyntaxKind.StarStarToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.StarStarToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));
        }
    }

    private static void DeclareBitwiseOperators(params ReadOnlySpan<TypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.TildeToken),
                    Name: Mangler.Mangle(SyntaxKind.TildeToken, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.TildeToken),
                        Parameters: [type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AmpersandToken),
                    Name: Mangler.Mangle(SyntaxKind.AmpersandToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AmpersandToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PipeToken),
                    Name: Mangler.Mangle(SyntaxKind.PipeToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PipeToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.HatToken),
                    Name: Mangler.Mangle(SyntaxKind.HatToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.HatToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanLessThanToken),
                    Name: Mangler.Mangle(SyntaxKind.LessThanLessThanToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanLessThanToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanGreaterThanToken),
                    Name: Mangler.Mangle(SyntaxKind.GreaterThanGreaterThanToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanGreaterThanToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));
        }
    }

    private static void DeclareLogicalOperators(params ReadOnlySpan<TypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.BangToken),
                    Name: Mangler.Mangle(SyntaxKind.BangToken, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.BangToken),
                        Parameters: [type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AmpersandAmpersandToken),
                    Name: Mangler.Mangle(SyntaxKind.AmpersandAmpersandToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AmpersandAmpersandToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));

            Declare(
                type,
                new VariableSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PipePipeToken),
                    Name: Mangler.Mangle(SyntaxKind.PipePipeToken, type, type),
                    Type: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PipePipeToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingModule: type.ContainingModule,
                    Modifiers.Static | Modifiers.ReadOnly));
        }
    }

    private static void DeclareStringOperators(TypeSymbol type)
    {
        Declare(
            type,
            new VariableSymbol(
                Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                Name: Mangler.Mangle(SyntaxKind.PlusToken, type, type),
                Type: new LambdaTypeSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    Parameters: [type, type],
                    ReturnType: type,
                    type.ContainingModule),
                ContainingModule: type.ContainingModule,
                Modifiers.Static | Modifiers.ReadOnly));

        Declare(
            type,
            new VariableSymbol(
                Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                Name: Mangler.Mangle(SyntaxKind.PlusToken, type, type.ContainingModule.Any),
                Type: new LambdaTypeSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    Parameters: [type, type.ContainingModule.Any],
                    ReturnType: type,
                    type.ContainingModule),
                ContainingModule: type.ContainingModule,
                Modifiers.Static | Modifiers.ReadOnly));

        Declare(
            type,
            new VariableSymbol(
                Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                Name: Mangler.Mangle(SyntaxKind.PlusToken, type.ContainingModule.Any, type),
                Type: new LambdaTypeSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    Parameters: [type.ContainingModule.Any, type],
                    ReturnType: type,
                    type.ContainingModule),
                ContainingModule: type.ContainingModule,
                Modifiers.Static | Modifiers.ReadOnly));
    }
}
