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
        DeclarePredefinedConversions(i8, i16, i32, i64, isz, u8, u16, u32, u64, usz, f16, f32, f64);

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

    private static void DeclareEqualityOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Equals,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.EqualsEqualsToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.EqualsEqualsToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.NotEquals,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.ExclamationEqualsToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.ExclamationEqualsToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingType: type));
        }
    }

    private static void DeclareComparisonOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LessThan,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LessThanOrEqual,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanEqualsToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanEqualsToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.GreaterThan,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.GreaterThanOrEqual,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanEqualsToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanEqualsToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingType: type));
        }
    }

    private static void DeclareMathOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.UnaryPlus,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                        Parameters: [type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.UnaryMinus,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.MinusToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.MinusToken),
                        Parameters: [type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Addition,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Subtraction,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.MinusToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.MinusToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Multiplication,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AsteriskToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AsteriskToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Division,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.SlashToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.SlashToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Modulo,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PercentToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PercentToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Exponentiation,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AsteriskAsteriskToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AsteriskAsteriskToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));
        }
    }

    private static void DeclareBitwiseOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.OnesComplement,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.TildeToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.TildeToken),
                        Parameters: [type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.BitwiseAnd,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AmpersandToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AmpersandToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.BitwiseOr,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.BarToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.BarToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.ExclusiveOr,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.CaretToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.CaretToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LeftShift,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanLessThanToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.LessThanLessThanToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.RightShift,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanGreaterThanToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.GreaterThanGreaterThanToken),
                        Parameters: [type, type],
                        ReturnType: type,
                        type.ContainingModule),
                    ContainingType: type));
        }
    }

    private static void DeclareLogicalOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.NotEquals,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.ExclamationToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.ExclamationToken),
                        Parameters: [type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LogicalAnd,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AmpersandAmpersandToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AmpersandAmpersandToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingType: type));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LogicalOr,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.BarBarToken),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.BarBarToken),
                        Parameters: [type, type],
                        ReturnType: type.ContainingModule.Bool,
                        type.ContainingModule),
                    ContainingType: type));
        }
    }

    private static void DeclareStringOperators(StructTypeSymbol type)
    {
        Declare(
            type,
            new OperatorSymbol(
                OperatorKind.Addition,
                Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                LambdaType: new LambdaTypeSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    Parameters: [type, type],
                    ReturnType: type,
                    type.ContainingModule),
                ContainingType: type));

        Declare(
            type,
            new OperatorSymbol(
                OperatorKind.Addition,
                Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                LambdaType: new LambdaTypeSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    Parameters: [type, type.ContainingModule.Any],
                    ReturnType: type,
                    type.ContainingModule),
                ContainingType: type));

        Declare(
            type,
            new OperatorSymbol(
                OperatorKind.Addition,
                Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                LambdaType: new LambdaTypeSymbol(
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.PlusToken),
                    Parameters: [type.ContainingModule.Any, type],
                    ReturnType: type,
                    type.ContainingModule),
                ContainingType: type));
    }

    private static void DeclarePredefinedConversions(
        StructTypeSymbol i8,
        StructTypeSymbol i16,
        StructTypeSymbol i32,
        StructTypeSymbol i64,
        StructTypeSymbol isz,
        StructTypeSymbol u8,
        StructTypeSymbol u16,
        StructTypeSymbol u32,
        StructTypeSymbol u64,
        StructTypeSymbol usz,
        StructTypeSymbol f16,
        StructTypeSymbol f32,
        StructTypeSymbol f64)
    {
        DeclareConversions(i8, [i16, i32, i64, isz, f16, f32, f64], [u8, u16, u32, u64]);
        DeclareConversions(i16, [i32, i64, isz, f16, f32, f64], [i8, u8, u16, u32, u64]);
        DeclareConversions(i32, [i64, isz, f32, f64], [f16, i8, i16, u8, u16, u32, u64]);
        DeclareConversions(i64, [isz, f32, f64], [f16, i8, i16, i32, u8, u16, u32, u64]);
        DeclareConversions(isz, [], [f16, f32, f64, i8, i16, i32, i64, u8, u16, u32, u64, usz]);
        DeclareConversions(u8, [u16, u32, u64, usz, f16, f32, f64], [i8, i16, i32, i64, isz]);
        DeclareConversions(u16, [u32, u64, usz, f16, f32, f64], [i8, i16, i32, i64, isz, u8]);
        DeclareConversions(u32, [u64, usz, f32, f64], [f16, i8, i16, i32, i64, isz, u8, u16]);
        DeclareConversions(u64, [usz, f32, f64], [f16, i8, i16, i32, i64, isz, u8, u16, u32]);
        DeclareConversions(usz, [], [f16, f32, f64, i8, i16, i32, i64, isz, u8, u16, u32, u64]);
        DeclareConversions(f16, [f32, f64], [i8, i16, i32, i64, isz, u8, u16, u32, u64, usz]);
        DeclareConversions(f32, [f64], [f16, i8, i16, i32, i64, isz, u8, u16, u32, u64, usz]);
        DeclareConversions(f64, [], [f16, f32, i8, i16, i32, i64, isz, u8, u16, u32, u64, usz]);
    }

    private static void DeclareConversions(
        StructTypeSymbol source,
        ReadOnlySpan<StructTypeSymbol> implicitTargets,
        ReadOnlySpan<StructTypeSymbol> explicitTargets)
    {
        DeclareConversions(source, ConversionKind.Implicit, implicitTargets);
        DeclareConversions(source, ConversionKind.Explicit, explicitTargets);
    }

    private static void DeclareConversions(
        StructTypeSymbol source,
        ConversionKind conversionKind,
        ReadOnlySpan<StructTypeSymbol> targets)
    {
        foreach (var target in targets)
        {
            Declare(
                source,
                new ConversionSymbol(
                    conversionKind,
                    Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AsKeyword),
                    LambdaType: new LambdaTypeSymbol(
                        Syntax: SyntaxToken.CreateSynthetic(SyntaxKind.AsKeyword),
                        Parameters: [source],
                        ReturnType: target,
                        source.ContainingModule),
                    ContainingType: source));
        }
    }
}
