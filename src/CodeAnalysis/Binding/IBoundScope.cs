using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal interface IBoundScope
{
    IBoundScope Parent { get; }
    ModuleSymbol Module { get; }
    bool Declare(Symbol symbol);
    Symbol? Lookup(string name);
    bool Replace(Symbol symbol);

    public StructTypeSymbol RuntimeType => Module.Lookup("type") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'type'");
    public StructTypeSymbol Any => Module.Lookup("any") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'any'");
    public StructTypeSymbol Err => Module.Lookup("err") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'err'");
    public StructTypeSymbol Unknown => Module.Lookup("unknown") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'unknown'");
    public StructTypeSymbol Never => Module.Lookup("never") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'never'");
    public StructTypeSymbol Unit => Module.Lookup("unit") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'unit'");
    public StructTypeSymbol Str => Module.Lookup("str") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'str'");
    public StructTypeSymbol Bool => Module.Lookup("bool") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'bool'");
    public StructTypeSymbol I8 => Module.Lookup("i8") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i8'");
    public StructTypeSymbol I16 => Module.Lookup("i16") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i16'");
    public StructTypeSymbol I32 => Module.Lookup("i32") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i32'");
    public StructTypeSymbol I64 => Module.Lookup("i64") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i64'");
    public StructTypeSymbol I128 => Module.Lookup("i128") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'i128'");
    public StructTypeSymbol Isz => Module.Lookup("isz") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'isz'");
    public StructTypeSymbol U8 => Module.Lookup("u8") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u8'");
    public StructTypeSymbol U16 => Module.Lookup("u16") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u16'");
    public StructTypeSymbol U32 => Module.Lookup("u32") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u32'");
    public StructTypeSymbol U64 => Module.Lookup("u64") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u64'");
    public StructTypeSymbol U128 => Module.Lookup("u128") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'u128'");
    public StructTypeSymbol Usz => Module.Lookup("usz") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'usz'");
    public StructTypeSymbol F16 => Module.Lookup("f16") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f16'");
    public StructTypeSymbol F32 => Module.Lookup("f32") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f32'");
    public StructTypeSymbol F64 => Module.Lookup("f64") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f64'");
    public StructTypeSymbol F80 => Module.Lookup("f80") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f80'");
    public StructTypeSymbol F128 => Module.Lookup("f128") as StructTypeSymbol
        ?? throw new UnreachableException($"Undeclared predefined symbol 'f128'");

    public static IBoundScope CreateGlobalScope()
    {
        // Modules:
        var global = MakeGlobalModule();

        // Types:
        var type = MakeRuntimeType(global);
        var any = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.AnyKeyword), "any", type, global);
        var err = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.ErrKeyword), "err", type, global);
        var unknown = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.UnknownKeyword), "unknown", type, global);
        var never = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.NeverKeyword), "never", type, global);
        var unit = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.UnitKeyword), "unit", type, global);
        var str = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.StrKeyword), "str", type, global);
        var @bool = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.BoolKeyword), "bool", type, global);
        var i8 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I8Keyword), "i8", type, global);
        var i16 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I16Keyword), "i16", type, global);
        var i32 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I32Keyword), "i32", type, global);
        var i64 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I64Keyword), "i64", type, global);
        var i128 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.I128Keyword), "i128", type, global);
        var isz = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.IszKeyword), "isz", type, global);
        var u8 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U8Keyword), "u8", type, global);
        var u16 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U16Keyword), "u16", type, global);
        var u32 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U32Keyword), "u32", type, global);
        var u64 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U64Keyword), "u64", type, global);
        var u128 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.U128Keyword), "u128", type, global);
        var usz = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.UszKeyword), "usz", type, global);
        var f16 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F16Keyword), "f16", type, global);
        var f32 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F32Keyword), "f32", type, global);
        var f64 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F64Keyword), "f64", type, global);
        var f80 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F80Keyword), "f80", type, global);
        var f128 = new StructTypeSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.F128Keyword), "f128", type, global);

        // TODO: Declare this int the fmt module.
        //// Functions:
        //var Print = new VariableSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), "print",
        //    new LambdaTypeSymbol([new Parameter("obj", Any)], Unit, GlobalModule), GlobalModule, IsStatic: true, IsReadOnly: true);
        //var Scan = new VariableSymbol(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), "scan",
        //    new LambdaTypeSymbol([], Str, GlobalModule), GlobalModule, IsStatic: true, IsReadOnly: true);

        // Type is it's own type. We need to avoid recursion.
        SetType(type, type);

        // Global module is contained within itself. We need to avoid recursion.
        SetContainingModule(global, global);
        // Global module type `module` is contained within the global module. We need to avoid recursion.
        SetType(global, never);
        // All predefined symbols exist within the global module.

        ReadOnlySpan<TypeSymbol> all = [
            type,
            any,
            err,
            unknown,
            never,
            unit,
            str,
            @bool,
            i8,
            i16,
            i32,
            i64,
            i128,
            isz,
            u8,
            u16,
            u32,
            u64,
            u128,
            usz,
            f16,
            f32,
            f64,
            f80,
            f128,
        ];

        foreach (var symbol in all)
            if (!global.Declare(symbol))
                throw new UnreachableException($"Failed to declare global symbol '{symbol}'");

        err
            .AddProperty("msg", str, isStatic: false, isReadOnly: true);

        str
            .AddEqualityOperators(global, type, @bool)
            .AddMembers(type =>
            {
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", str), new Parameter("y", str)], str, type, global));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", str), new Parameter("y", any)], str, type, global));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", any), new Parameter("y", str)], str, type, global));
            });

        @bool
            .AddEqualityOperators(global, type, @bool)
            .AddLogicalOperators(global, type, @bool);

        i8
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, i16, i32, i64, i128, isz, f16, f32, f64, f80, f128)
                .AddExplicitConversion(global, type, u8, u16, u32, u64, u128, usz);

        i16
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, i32, i64, i128, isz, f16, f32, f64, f80, f128)
                .AddExplicitConversion(global, type, i8, u8, u16, u32, u64, u128, usz);

        i32
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, i64, i128, isz, f32, f64, f80, f128)
                .AddExplicitConversion(global, type, i8, i16, u8, u16, u32, u64, u128, usz, f16);

        i64
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, i128, isz, f32, f64, f80, f128)
                .AddExplicitConversion(global, type, i8, i16, i32, u8, u16, u32, u64, u128, usz, f16);

        i128
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddExplicitConversion(global, type, i8, i16, i32, i64, isz, u8, u16, u32, u64, u128, usz, f16, f32, f64, f80, f128);

        isz
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddExplicitConversion(global, type, i8, i16, i32, i64, i128, u8, u16, u32, u64, u128, usz, f16, f32, f64, f80, f128);


        u8
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, u16, u32, u64, u128, usz, f16, f32, f64, f80, f128)
                .AddExplicitConversion(global, type, i8, i16, i32, i64, i128, isz);

        u16
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, u32, u64, u128, usz, f16, f32, f64, f80, f128)
                .AddExplicitConversion(global, type, u8, i8, i16, i32, i64, i128, isz);

        u32
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, u64, u128, usz, f32, f64, f80, f128)
                .AddExplicitConversion(global, type, u8, u16, i8, i16, i32, i64, i128, isz, f16);

        u64
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, u128, usz, f32, f64, f80, f128)
                .AddExplicitConversion(global, type, u8, u16, u32, i8, i16, i32, i64, i128, isz, f16);

        u128
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddExplicitConversion(global, type, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, usz, f16, f32, f64, f80, f128);

        usz
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddBitwiseOperators(global, type)
                .AddMathOperators(global, type)
                .AddExplicitConversion(global, type, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, f16, f32, f64, f80, f128);

        f16
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, f32, f64, f80, f128)
                .AddExplicitConversion(global, type, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

        f32
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, f64, f80, f128)
                .AddExplicitConversion(global, type, f16, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

        f64
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, f80, f128)
                .AddExplicitConversion(global, type, f16, f32, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

        f80
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddMathOperators(global, type)
                .AddImplicitConversion(global, type, f128)
                .AddExplicitConversion(global, type, f16, f32, f64, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

        f128
            .AddEqualityOperators(global, type, @bool)
                .AddComparisonOperators(global, type, @bool)
                .AddMathOperators(global, type)
                .AddExplicitConversion(global, type, f16, f32, f64, f80, i8, i16, i32, i64, i128, isz, u8, u16, u32, u64, u128, usz);

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
}

file static class GlobalScopeFactory
{
    public static TypeSymbol AddMembers(this TypeSymbol type, Action<TypeSymbol> add)
    {
        add(type);
        return type;
    }

    public static TypeSymbol AddMathOperators(this TypeSymbol type, ModuleSymbol global, TypeSymbol runtimeType)
    {
        type.AddOperator(
            SyntaxKind.PlusToken,
            new LambdaTypeSymbol([new("x", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.MinusToken,
            new LambdaTypeSymbol([new("x", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.PlusToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.MinusToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.StarToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.SlashToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.PercentToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.StarStarToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        return type;
    }

    public static TypeSymbol AddBitwiseOperators(this TypeSymbol type, ModuleSymbol global, TypeSymbol runtimeType)
    {
        type.AddOperator(
            SyntaxKind.TildeToken,
            new LambdaTypeSymbol([new("x", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.AmpersandToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.PipeToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.HatToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.LessThanLessThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        type.AddOperator(
            SyntaxKind.GreaterThanGreaterThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, runtimeType, global));
        return type;
    }

    public static TypeSymbol AddEqualityOperators(this TypeSymbol type, ModuleSymbol global, TypeSymbol runtimeType, TypeSymbol @bool)
    {
        type.AddOperator(
            SyntaxKind.EqualsEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], @bool, runtimeType, global));
        type.AddOperator(
            SyntaxKind.BangEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], @bool, runtimeType, global));
        return type;
    }

    public static TypeSymbol AddComparisonOperators(this TypeSymbol type, ModuleSymbol global, TypeSymbol runtimeType, TypeSymbol @bool)
    {
        type.AddOperator(
            SyntaxKind.LessThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], @bool, runtimeType, global));
        type.AddOperator(
            SyntaxKind.LessThanEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], @bool, runtimeType, global));
        type.AddOperator(
            SyntaxKind.GreaterThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], @bool, runtimeType, global));
        type.AddOperator(
            SyntaxKind.GreaterThanEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], @bool, runtimeType, global));
        return type;
    }

    public static TypeSymbol AddLogicalOperators(this TypeSymbol type, ModuleSymbol global, TypeSymbol runtimeType, TypeSymbol @bool)
    {
        type.AddOperator(
            SyntaxKind.BangToken,
            new LambdaTypeSymbol([new("x", type)], @bool, runtimeType, global));
        type.AddOperator(
            SyntaxKind.AmpersandAmpersandToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], @bool, runtimeType, global));
        type.AddOperator(
            SyntaxKind.PipePipeToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], @bool, runtimeType, global));
        return type;
    }

    public static TypeSymbol AddImplicitConversion(this TypeSymbol type, ModuleSymbol global, TypeSymbol runtimeType, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ImplicitKeyword, new LambdaTypeSymbol([new Parameter("x", type)], targetType, runtimeType, global));
        return type;
    }

    public static TypeSymbol AddExplicitConversion(this TypeSymbol type, ModuleSymbol global, TypeSymbol runtimeType, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ExplicitKeyword, new LambdaTypeSymbol([new Parameter("x", type)], targetType, runtimeType, global));
        return type;
    }
}
