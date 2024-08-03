using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal interface IBoundScope
{
    StructTypeSymbol RuntimeType { get; }
    StructTypeSymbol ModuleType { get; }
    StructTypeSymbol Any { get; }
    StructTypeSymbol Err { get; }
    StructTypeSymbol Unknown { get; }
    StructTypeSymbol Never { get; }
    StructTypeSymbol Unit { get; }
    StructTypeSymbol Str { get; }
    StructTypeSymbol Bool { get; }
    StructTypeSymbol I8 { get; }
    StructTypeSymbol I16 { get; }
    StructTypeSymbol I32 { get; }
    StructTypeSymbol I64 { get; }
    StructTypeSymbol I128 { get; }
    StructTypeSymbol Isz { get; }
    StructTypeSymbol U8 { get; }
    StructTypeSymbol U16 { get; }
    StructTypeSymbol U32 { get; }
    StructTypeSymbol U64 { get; }
    StructTypeSymbol U128 { get; }
    StructTypeSymbol Usz { get; }
    StructTypeSymbol F16 { get; }
    StructTypeSymbol F32 { get; }
    StructTypeSymbol F64 { get; }
    StructTypeSymbol F80 { get; }
    StructTypeSymbol F128 { get; }

    IBoundScope Parent { get; }
    ModuleSymbol Module { get; }
    bool Declare(Symbol symbol);
    Symbol? Lookup(string name);
    bool Replace(Symbol symbol);

    bool DeclareModule(string name, out ModuleSymbol moduleSymbol);
    bool DeclareModule(SyntaxNode syntax, string name, out ModuleSymbol moduleSymbol);

    bool DeclareStruct(string name, out StructTypeSymbol structTypeSymbol);
    bool DeclareStruct(SyntaxNode syntax, string name, out StructTypeSymbol structTypeSymbol);

    bool DeclareVariable(string name, TypeSymbol type, bool isStatic, bool isReadOnly, out VariableSymbol variableSymbol);
    bool DeclareVariable(SyntaxNode syntax, string name, TypeSymbol type, bool isStatic, bool isReadOnly, out VariableSymbol variableSymbol);

    ArrayTypeSymbol CreateArrayType(TypeSymbol elementType, int length, SyntaxNode? syntax = null);
    ErrorTypeSymbol CreateErrorType(TypeSymbol valueType, SyntaxNode? syntax = null);
    LambdaTypeSymbol CreateLambdaType(IEnumerable<Parameter> parameters, TypeSymbol returnType, SyntaxNode? syntax = null);
    OptionTypeSymbol CreateOptionType(TypeSymbol underlyingType, SyntaxNode? syntax = null);
    PointerTypeSymbol CreatePointerType(TypeSymbol elementType, SyntaxNode? syntax = null);
    StructTypeSymbol CreateStructType(string name, SyntaxNode? syntax = null);
    UnionTypeSymbol CreateUnionType(IEnumerable<TypeSymbol> types, SyntaxNode? syntax = null);

    public static IBoundScope CreateGlobalScope()
    {
        var global = Factory.CreateGlobalModule();

        Factory.DeclareFmtModule(global);

        return global;
    }
}

file static class Factory
{
    public static ModuleSymbol CreateGlobalModule()
    {
        var global = MakeGlobalModule();
        var type = MakeRuntimeType(global);

        ReadOnlySpan<bool> all = [
            global.Declare(type),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.ModuleKeyword), "module", out var module),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.AnyKeyword), "any", out var any),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.ErrKeyword), "err", out var err),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.UnknownKeyword), "unknown", out var unknown),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.NeverKeyword), "never", out var never),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.UnitKeyword), "unit", out var unit),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.StrKeyword), "str", out var str),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.BoolKeyword), "bool", out var @bool),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.I8Keyword), "i8", out var i8),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.I16Keyword), "i16", out var i16),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.I32Keyword), "i32", out var i32),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.I64Keyword), "i64", out var i64),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.I128Keyword), "i128", out var i128),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.IszKeyword), "isz", out var isz),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.U8Keyword), "u8", out var u8),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.U16Keyword), "u16", out var u16),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.U32Keyword), "u32", out var u32),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.U64Keyword), "u64", out var u64),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.U128Keyword), "u128", out var u128),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.UszKeyword), "usz", out var usz),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.F16Keyword), "f16", out var f16),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.F32Keyword), "f32", out var f32),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.F64Keyword), "f64", out var f64),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.F80Keyword), "f80", out var f80),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.F128Keyword), "f128", out var f128),
        ];

        if (all.IndexOf(false) is var index && index > 0)
            throw new UnreachableException($"Failed to declare global symbol at index '{index}'");

        // Type is it's own type. We need to avoid recursion.
        SetType(type, type);

        // Global module is contained within itself. We need to avoid recursion.
        SetContainingModule(global, global);
        // Global module type `module` is contained within the global module. We need to avoid recursion.
        SetType(global, module);
        // All predefined symbols exist within the global module.

        err
            .AddProperty("msg", str, isStatic: false, isReadOnly: true);

        str
            .AddEqualityOperators()
            .AddMembers(type =>
            {
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    global.CreateLambdaType([new Parameter("x", str), new Parameter("y", str)], str));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    global.CreateLambdaType([new Parameter("x", str), new Parameter("y", any)], str));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    global.CreateLambdaType([new Parameter("x", any), new Parameter("y", str)], str));
            });

        @bool
            .AddEqualityOperators()
            .AddLogicalOperators();

        i8
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(i16, i32, i64, i128, isz, f16, f32, f64, f80, f128)
            .AddExplicitConversion(u8, u16, u32, u64, u128, usz);

        i16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(i32, i64, i128, isz, f16, f32, f64, f80, f128)
            .AddExplicitConversion(i8, u8, u16, u32, u64, u128, usz);

        i32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(i64, i128, isz, f32, f64, f80, f128)
            .AddExplicitConversion(i8, i16, u8, u16, u32, u64, u128, usz, f16);

        i64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(i128, isz, f32, f64, f80, f128)
            .AddExplicitConversion(i8, i16, i32, u8, u16, u32, u64, u128, usz, f16);

        i128
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(i8, i16, i32, i64, isz, u8, u16, u32, u64, u128, usz, f16, f32, f64, f80, f128);

        isz
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(i8, i16, i32, i64, i128, u8, u16, u32, u64, u128, usz, f16, f32, f64, f80, f128);


        u8
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(u16, u32, u64, u128, usz, f16, f32, f64, f80, f128)
            .AddExplicitConversion(i8, i16, i32, i64, i128, isz);

        u16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(u32, u64, u128, usz, f16, f32, f64, f80, f128)
            .AddExplicitConversion(u8, i8, i16, i32, i64, i128, isz);

        u32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(u64, u128, usz, f32, f64, f80, f128)
            .AddExplicitConversion(u8, u16, i8, i16, i32, i64, i128, isz, f16);

        u64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(u128, usz, f32, f64, f80, f128)
            .AddExplicitConversion(u8, u16, u32, i8, i16, i32, i64, i128, isz, f16);

        u128
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, usz, f16, f32, f64, f80, f128);

        usz
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, f16, f32, f64, f80, f128);

        f16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(f32, f64, f80, f128)
            .AddExplicitConversion(i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

        f32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(f64, f80, f128)
            .AddExplicitConversion(f16, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

        f64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(f80, f128)
            .AddExplicitConversion(f16, f32, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

        f80
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(f128)
            .AddExplicitConversion(f16, f32, f64, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

        f128
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddExplicitConversion(f16, f32, f64, f80, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

        return global;

        // Helper functions.
        static ModuleSymbol MakeGlobalModule()
        {
            var module = (ModuleSymbol)RuntimeHelpers.GetUninitializedObject(typeof(ModuleSymbol)) with
            {
                BoundKind = BoundKind.ModuleSymbol,
                Syntax = SyntaxFactory.SyntheticToken(SyntaxKind.ModuleKeyword),
                Name = "<global>",
                Type = null!,
                ContainingModule = null!,
                IsStatic = true,
                IsReadOnly = true,
            };

            ref var symbols = ref GetSymbolsFieldRef(module);
            symbols = [];

            return module;

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_symbols")]
            extern static ref Dictionary<string, Symbol> GetSymbolsFieldRef(ModuleSymbol module);

        }

        static StructTypeSymbol MakeRuntimeType(ModuleSymbol globalModule)
        {
            var type = (StructTypeSymbol)RuntimeHelpers.GetUninitializedObject(typeof(StructTypeSymbol)) with
            {
                BoundKind = BoundKind.StructTypeSymbol,
                Syntax = SyntaxFactory.SyntheticToken(SyntaxKind.TypeKeyword),
                Name = "type",
                Type = null!,
                ContainingModule = globalModule,
                IsStatic = true,
                IsReadOnly = true,
            };

            ref var members = ref GetMembersFieldRef(type);
            members = [];

            return type;

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_members")]
            extern static ref List<Symbol> GetMembersFieldRef(TypeSymbol type);
        }

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_ContainingModule")]
        extern static void SetContainingModule(Symbol symbol, ModuleSymbol containingModule);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Type")]
        extern static void SetType(Symbol symbol, TypeSymbol type);
    }

    public static void DeclareFmtModule(ModuleSymbol global)
    {
        if (!global.DeclareModule("fmt", out _))
            throw new UnreachableException($"Failed to declare 'fmt' module");

        ReadOnlySpan<bool> all = [
            global.DeclareVariable(
                "print",
                global.CreateLambdaType([new Parameter("obj", global.Any)], global.Unit),
                isStatic: true,
                isReadOnly: true,
                out _),

            global.DeclareVariable(
                "scan",
                global.CreateLambdaType([], global.Str),
                isStatic: true,
                isReadOnly: true,
                out _),
        ];

        if (all.IndexOf(false) is var index && index > 0)
            throw new UnreachableException($"Failed to declare fmt symbol at index '{index}'");
    }

    private static TypeSymbol AddMembers(this TypeSymbol type, Action<TypeSymbol> add)
    {
        add(type);
        return type;
    }

    private static TypeSymbol AddMathOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.PlusToken,
            type.ContainingModule.CreateLambdaType([new("x", type)], type));
        type.AddOperator(
            SyntaxKind.MinusToken,
            type.ContainingModule.CreateLambdaType([new("x", type)], type));
        type.AddOperator(
            SyntaxKind.PlusToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.MinusToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.StarToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.SlashToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.PercentToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.StarStarToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        return type;
    }

    private static TypeSymbol AddBitwiseOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.TildeToken,
            type.ContainingModule.CreateLambdaType([new("x", type)], type));
        type.AddOperator(
            SyntaxKind.AmpersandToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.PipeToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.HatToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.LessThanLessThanToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.GreaterThanGreaterThanToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type));
        return type;
    }

    private static TypeSymbol AddEqualityOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.EqualsEqualsToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type.ContainingModule.Bool));
        type.AddOperator(
            SyntaxKind.BangEqualsToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type.ContainingModule.Bool));
        return type;
    }

    private static TypeSymbol AddComparisonOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.LessThanToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type.ContainingModule.Bool));
        type.AddOperator(
            SyntaxKind.LessThanEqualsToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type.ContainingModule.Bool));
        type.AddOperator(
            SyntaxKind.GreaterThanToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type.ContainingModule.Bool));
        type.AddOperator(
            SyntaxKind.GreaterThanEqualsToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type.ContainingModule.Bool));
        return type;
    }

    private static TypeSymbol AddLogicalOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.BangToken,
            type.ContainingModule.CreateLambdaType([new("x", type)], type.ContainingModule.Bool));
        type.AddOperator(
            SyntaxKind.AmpersandAmpersandToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type.ContainingModule.Bool));
        type.AddOperator(
            SyntaxKind.PipePipeToken,
            type.ContainingModule.CreateLambdaType([new("x", type), new("y", type)], type.ContainingModule.Bool));
        return type;
    }

    private static TypeSymbol AddImplicitConversion(this TypeSymbol type, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ImplicitKeyword, type.ContainingModule.CreateLambdaType([new Parameter("x", type)], targetType));
        return type;
    }

    private static TypeSymbol AddExplicitConversion(this TypeSymbol type, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ExplicitKeyword, type.ContainingModule.CreateLambdaType([new Parameter("x", type)], targetType));
        return type;
    }
}
