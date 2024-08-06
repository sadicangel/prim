using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ModuleSymbol(
    SyntaxNode Syntax,
    string Name,
    ModuleSymbol ContainingModule)
    : ScopeSymbol(
        BoundKind.ModuleSymbol,
        Syntax,
        Name,
        ContainingModule.Never,
        ContainingModule,
        ContainingModule,
        IsStatic: true,
        IsReadOnly: true)
{
    private readonly Dictionary<string, Symbol> _symbols = [];

    public bool IsGlobal => Name == GlobalModule.Name;

    public override bool IsAnonymous => false;

    public override ModuleSymbol Module => this;

    public static ModuleSymbol CreateGlobalModule()
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
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.AnyKeyword), PredefinedTypes.Any, out var any),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.ErrKeyword), PredefinedTypes.Err, out var err),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.UnknownKeyword), PredefinedTypes.Unknown, out var unknown),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.NeverKeyword), PredefinedTypes.Never, out var never),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.UnitKeyword), PredefinedTypes.Unit, out var unit),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.StrKeyword), PredefinedTypes.Str, out var str),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.BoolKeyword), PredefinedTypes.Bool, out var @bool),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.I8Keyword), PredefinedTypes.I8, out var i8),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.I16Keyword), PredefinedTypes.I16, out var i16),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.I32Keyword), PredefinedTypes.I32, out var i32),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.I64Keyword), PredefinedTypes.I64, out var i64),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.IszKeyword), PredefinedTypes.Isz, out var isz),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.U8Keyword), PredefinedTypes.U8, out var u8),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.U16Keyword), PredefinedTypes.U16, out var u16),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.U32Keyword), PredefinedTypes.U32, out var u32),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.U64Keyword), PredefinedTypes.U64, out var u64),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.UszKeyword), PredefinedTypes.Usz, out var usz),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.F16Keyword), PredefinedTypes.F16, out var f16),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.F32Keyword), PredefinedTypes.F32, out var f32),
            global.DeclareStruct(SyntaxFactory.SyntheticToken(SyntaxKind.F64Keyword), PredefinedTypes.F64, out var f64),
        ];

        if (all.IndexOf(false) is var index && index > 0)
            throw new UnreachableException($"Failed to declare global symbol at index '{index}'");

        // Type is it's own type. We need to avoid recursion.
        SetType(type, type);

        // Global module type `module` is contained within the global module. We need to avoid recursion.
        SetType(global, never);
        // All predefined symbols exist within the global module.

        err
            .AddProperty("msg", str, isStatic: false, isReadOnly: true);

        str
            .AddEqualityOperators()
            .AddMembers(type =>
            {
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", str), new Parameter("y", str)], str, global));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", str), new Parameter("y", any)], str, global));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", any), new Parameter("y", str)], str, global));
            });

        @bool
            .AddEqualityOperators()
            .AddLogicalOperators();

        i8
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(i16, i32, i64, isz, f16, f32, f64)
            .AddExplicitConversion(u8, u16, u32, u64, usz);

        i16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(i32, i64, isz, f16, f32, f64)
            .AddExplicitConversion(i8, u8, u16, u32, u64, usz);

        i32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(i64, isz, f32, f64)
            .AddExplicitConversion(i8, i16, u8, u16, u32, u64, usz, f16);

        i64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(isz, f32, f64)
            .AddExplicitConversion(i8, i16, i32, u8, u16, u32, u64, usz, f16);

        isz
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(i8, i16, i32, i64, u8, u16, u32, u64, usz, f16, f32, f64);


        u8
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(u16, u32, u64, usz, f16, f32, f64)
            .AddExplicitConversion(i8, i16, i32, i64, isz);

        u16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(u32, u64, usz, f16, f32, f64)
            .AddExplicitConversion(u8, i8, i16, i32, i64, isz);

        u32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(u64, usz, f32, f64)
            .AddExplicitConversion(u8, u16, i8, i16, i32, i64, isz, f16);

        u64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(usz, f32, f64)
            .AddExplicitConversion(u8, u16, u32, i8, i16, i32, i64, isz, f16);

        usz
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(i8, i16, i32, i64, isz, u8, u16, u32, u64, f16, f32, f64);

        f16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(f32, f64)
            .AddExplicitConversion(i8, i16, i32, i64, isz, u8, u16, u32, u64, usz);

        f32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(f64)
            .AddExplicitConversion(f16, i8, i16, i32, i64, isz, u8, u16, u32, u64, usz);

        f64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddExplicitConversion(f16, f32, i8, i16, i32, i64, isz, u8, u16, u32, u64, usz);

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

            SetContainingModule(module, module);

            return module;

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_symbols")]
            extern static ref Dictionary<string, Symbol> GetSymbolsFieldRef(ModuleSymbol module);

            [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(Symbol.ContainingModule)}")]
            extern static void SetContainingModule(Symbol symbol, ModuleSymbol containingModule);

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

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(TypeSymbol.Type)}")]
        extern static void SetType(Symbol symbol, TypeSymbol type);
    }

    public static void DeclareFmtModule(ModuleSymbol global)
    {
        if (!global.DeclareModule("fmt", out _))
            throw new UnreachableException($"Failed to declare 'fmt' module");

        ReadOnlySpan<bool> all = [
            global.DeclareVariable(
                "print",
                new LambdaTypeSymbol([new Parameter("obj", global.Any)], global.Unit, global),
                isStatic: true,
                isReadOnly: true,
                out _),

            global.DeclareVariable(
                "scan",
                new LambdaTypeSymbol([], global.Str, global),
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
            new LambdaTypeSymbol([new("x", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.MinusToken,
            new LambdaTypeSymbol([new("x", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.PlusToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.MinusToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.StarToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.SlashToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.PercentToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.StarStarToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        return type;
    }

    private static TypeSymbol AddBitwiseOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.TildeToken,
            new LambdaTypeSymbol([new("x", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.AmpersandToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.PipeToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.HatToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.LessThanLessThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.GreaterThanGreaterThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, type.ContainingModule));
        return type;
    }

    private static TypeSymbol AddEqualityOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.EqualsEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type.ContainingModule.Bool, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.BangEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type.ContainingModule.Bool, type.ContainingModule));
        return type;
    }

    private static TypeSymbol AddComparisonOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.LessThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type.ContainingModule.Bool, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.LessThanEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type.ContainingModule.Bool, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.GreaterThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type.ContainingModule.Bool, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.GreaterThanEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type.ContainingModule.Bool, type.ContainingModule));
        return type;
    }

    private static TypeSymbol AddLogicalOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.BangToken,
            new LambdaTypeSymbol([new("x", type)], type.ContainingModule.Bool, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.AmpersandAmpersandToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type.ContainingModule.Bool, type.ContainingModule));
        type.AddOperator(
            SyntaxKind.PipePipeToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type.ContainingModule.Bool, type.ContainingModule));
        return type;
    }

    private static TypeSymbol AddImplicitConversion(this TypeSymbol type, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ImplicitKeyword, new LambdaTypeSymbol([new Parameter("x", type)], targetType, type.ContainingModule));
        return type;
    }

    private static TypeSymbol AddExplicitConversion(this TypeSymbol type, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ExplicitKeyword, new LambdaTypeSymbol([new Parameter("x", type)], targetType, type.ContainingModule));
        return type;
    }
}
