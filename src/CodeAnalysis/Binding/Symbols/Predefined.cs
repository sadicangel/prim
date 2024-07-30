using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

// TODO: Merge this into global scope.
internal static class Predefined
{
    // Modules:
    private static ModuleSymbol MakeGlobalModule()
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
    public static readonly ModuleSymbol GlobalModule = MakeGlobalModule();

    private static StructTypeSymbol MakeRuntimeType()
    {
        var type = (StructTypeSymbol)RuntimeHelpers.GetUninitializedObject(typeof(StructTypeSymbol)) with
        {
            BoundKind = BoundKind.StructTypeSymbol,
            Syntax = SyntaxFactory.SyntheticToken(SyntaxKind.TypeKeyword),
            Name = "type",
            Type = null!,
            ContainingModule = GlobalModule,
            IsStatic = true,
            IsReadOnly = true,
        };

        return type;
    }

    // Types:
    public static readonly StructTypeSymbol Type = MakeRuntimeType();
    public static readonly StructTypeSymbol Any = new(SyntaxFactory.SyntheticToken(SyntaxKind.AnyKeyword), "any", GlobalModule);
    public static readonly StructTypeSymbol Err = new(SyntaxFactory.SyntheticToken(SyntaxKind.ErrKeyword), "err", GlobalModule);
    public static readonly StructTypeSymbol Unknown = new(SyntaxFactory.SyntheticToken(SyntaxKind.UnknownKeyword), "unknown", GlobalModule);
    public static readonly StructTypeSymbol Never = new(SyntaxFactory.SyntheticToken(SyntaxKind.NeverKeyword), "never", GlobalModule);
    public static readonly StructTypeSymbol Unit = new(SyntaxFactory.SyntheticToken(SyntaxKind.UnitKeyword), "unit", GlobalModule);
    public static readonly StructTypeSymbol Str = new(SyntaxFactory.SyntheticToken(SyntaxKind.StrKeyword), "str", GlobalModule);
    public static readonly StructTypeSymbol Bool = new(SyntaxFactory.SyntheticToken(SyntaxKind.BoolKeyword), "bool", GlobalModule);
    public static readonly StructTypeSymbol I8 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I8Keyword), "i8", GlobalModule);
    public static readonly StructTypeSymbol I16 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I16Keyword), "i16", GlobalModule);
    public static readonly StructTypeSymbol I32 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I32Keyword), "i32", GlobalModule);
    public static readonly StructTypeSymbol I64 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I64Keyword), "i64", GlobalModule);
    public static readonly StructTypeSymbol I128 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I128Keyword), "i128", GlobalModule);
    public static readonly StructTypeSymbol ISize = new(SyntaxFactory.SyntheticToken(SyntaxKind.ISizeKeyword), "isize", GlobalModule);
    public static readonly StructTypeSymbol U8 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U8Keyword), "u8", GlobalModule);
    public static readonly StructTypeSymbol U16 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U16Keyword), "u16", GlobalModule);
    public static readonly StructTypeSymbol U32 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U32Keyword), "u32", GlobalModule);
    public static readonly StructTypeSymbol U64 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U64Keyword), "u64", GlobalModule);
    public static readonly StructTypeSymbol U128 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U128Keyword), "u128", GlobalModule);
    public static readonly StructTypeSymbol USize = new(SyntaxFactory.SyntheticToken(SyntaxKind.USizeKeyword), "usize", GlobalModule);
    public static readonly StructTypeSymbol F16 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F16Keyword), "f16", GlobalModule);
    public static readonly StructTypeSymbol F32 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F32Keyword), "f32", GlobalModule);
    public static readonly StructTypeSymbol F64 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F64Keyword), "f64", GlobalModule);
    public static readonly StructTypeSymbol F80 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F80Keyword), "f80", GlobalModule);
    public static readonly StructTypeSymbol F128 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F128Keyword), "f128", GlobalModule);

    // Functions:
    public static readonly VariableSymbol Print =
        new(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), "print", new LambdaTypeSymbol([new Parameter("obj", Any)], Unit, GlobalModule), GlobalModule, IsStatic: true, IsReadOnly: true);
    public static readonly VariableSymbol Scan =
        new(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), "scan", new LambdaTypeSymbol([], Str, GlobalModule), GlobalModule, IsStatic: true, IsReadOnly: true);

    static Predefined()
    {
        // Type is it's own type. We need to avoid recursion.
        SetType(Type, Type);

        // Global module is contained within itself. We need to avoid recursion.
        SetContainingModule(GlobalModule, GlobalModule);
        // Global module type `module` is contained within the global module. We need to avoid recursion.
        SetType(GlobalModule, Never);
        // All predefined symbols exist within the global module.
        foreach (var symbol in All())
            if (!GlobalModule.Declare(symbol))
                throw new UnreachableException($"Failed to declare global symbol '{symbol}'");

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_ContainingModule")]
        extern static void SetContainingModule(Symbol symbol, ModuleSymbol containingModule);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Type")]
        extern static void SetType(Symbol symbol, TypeSymbol type);

        Err
            .AddProperty("msg", Str, isStatic: false, isReadOnly: true);

        Str
            .AddEqualityOperators()
            .AddMembers(type =>
            {
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", Str), new Parameter("y", Str)], Str, GlobalModule));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", Str), new Parameter("y", Any)], Str, GlobalModule));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", Any), new Parameter("y", Str)], Str, GlobalModule));
            });

        Bool
            .AddEqualityOperators()
            .AddLogicalOperators();

        I8
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(I16, I32, I64, I128, ISize, F16, F32, F64, F80, F128)
            .AddExplicitConversion(U8, U16, U32, U64, U128, USize);

        I16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(I32, I64, I128, ISize, F16, F32, F64, F80, F128)
            .AddExplicitConversion(I8, U8, U16, U32, U64, U128, USize);

        I32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(I64, I128, ISize, F32, F64, F80, F128)
            .AddExplicitConversion(I8, I16, U8, U16, U32, U64, U128, USize, F16);

        I64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(I128, ISize, F32, F64, F80, F128)
            .AddExplicitConversion(I8, I16, I32, U8, U16, U32, U64, U128, USize, F16);

        I128
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(I8, I16, I32, I64, ISize, U8, U16, U32, U64, U128, USize, F16, F32, F64, F80, F128);

        ISize
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(I8, I16, I32, I64, I128, U8, U16, U32, U64, U128, USize, F16, F32, F64, F80, F128);


        U8
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(U16, U32, U64, U128, USize, F16, F32, F64, F80, F128)
            .AddExplicitConversion(I8, I16, I32, I64, I128, ISize);

        U16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(U32, U64, U128, USize, F16, F32, F64, F80, F128)
            .AddExplicitConversion(U8, I8, I16, I32, I64, I128, ISize);

        U32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(U64, U128, USize, F32, F64, F80, F128)
            .AddExplicitConversion(U8, U16, I8, I16, I32, I64, I128, ISize, F16);

        U64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(U128, USize, F32, F64, F80, F128)
            .AddExplicitConversion(U8, U16, U32, I8, I16, I32, I64, I128, ISize, F16);

        U128
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, USize, F16, F32, F64, F80, F128);

        USize
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, F16, F32, F64, F80, F128);

        F16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(F32, F64, F80, F128)
            .AddExplicitConversion(I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);

        F32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(F64, F80, F128)
            .AddExplicitConversion(F16, I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);

        F64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(F80, F128)
            .AddExplicitConversion(F16, F32, I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);

        F80
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(F128)
            .AddExplicitConversion(F16, F32, F64, I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);

        F128
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddExplicitConversion(F16, F32, F64, F80, I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);
    }

    public static IEnumerable<Symbol> All() => typeof(Predefined)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (Symbol)f.GetValue(null)!);

    private static TypeSymbol AddMembers(this TypeSymbol type, Action<TypeSymbol> add)
    {
        add(type);
        return type;
    }

    private static TypeSymbol AddMathOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.PlusToken,
            new LambdaTypeSymbol([new("x", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.MinusToken,
            new LambdaTypeSymbol([new("x", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.PlusToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.MinusToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.StarToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.SlashToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.PercentToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.StarStarToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        return type;
    }

    private static TypeSymbol AddBitwiseOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.TildeToken,
            new LambdaTypeSymbol([new("x", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.AmpersandToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.PipeToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.HatToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.LessThanLessThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        type.AddOperator(
            SyntaxKind.GreaterThanGreaterThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type, GlobalModule));
        return type;
    }

    private static TypeSymbol AddEqualityOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.EqualsEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool, GlobalModule));
        type.AddOperator(
            SyntaxKind.BangEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool, GlobalModule));
        return type;
    }

    private static TypeSymbol AddComparisonOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.LessThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool, GlobalModule));
        type.AddOperator(
            SyntaxKind.LessThanEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool, GlobalModule));
        type.AddOperator(
            SyntaxKind.GreaterThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool, GlobalModule));
        type.AddOperator(
            SyntaxKind.GreaterThanEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool, GlobalModule));
        return type;
    }

    private static TypeSymbol AddLogicalOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.BangToken,
            new LambdaTypeSymbol([new("x", type)], Bool, GlobalModule));
        type.AddOperator(
            SyntaxKind.AmpersandAmpersandToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool, GlobalModule));
        type.AddOperator(
            SyntaxKind.PipePipeToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool, GlobalModule));
        return type;
    }

    private static TypeSymbol AddImplicitConversion(this TypeSymbol type, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ImplicitKeyword, new LambdaTypeSymbol([new Parameter("x", type)], targetType, GlobalModule));
        return type;
    }

    private static TypeSymbol AddExplicitConversion(this TypeSymbol type, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ExplicitKeyword, new LambdaTypeSymbol([new Parameter("x", type)], targetType, GlobalModule));
        return type;
    }
}

