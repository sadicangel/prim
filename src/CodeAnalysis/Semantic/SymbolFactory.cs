using System.Runtime.CompilerServices;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal static class SymbolFactory
{
    public static ModuleSymbol CreateGlobalModule()
    {
        var global = MakeGlobalModule();

        var module = Declare(global, MakeModuleType(global));
        var type = Declare(global, MakeTypeType(global));
        var never = Declare(global, new StructTypeSymbol(PredefinedTypeNames.Never, global));
        var any = Declare(global, new StructTypeSymbol(PredefinedTypeNames.Any, global));
        var err = Declare(global, new StructTypeSymbol(PredefinedTypeNames.Err, global));
        var unknown = Declare(global, new StructTypeSymbol(PredefinedTypeNames.Unknown, global));
        var unit = Declare(global, new StructTypeSymbol(PredefinedTypeNames.Unit, global));
        var str = Declare(global, new StructTypeSymbol(PredefinedTypeNames.Str, global));
        var @bool = Declare(global, new StructTypeSymbol(PredefinedTypeNames.Bool, global));
        var i8 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.I8, global));
        var i16 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.I16, global));
        var i32 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.I32, global));
        var i64 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.I64, global));
        var isz = Declare(global, new StructTypeSymbol(PredefinedTypeNames.Isz, global));
        var u8 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.U8, global));
        var u16 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.U16, global));
        var u32 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.U32, global));
        var u64 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.U64, global));
        var usz = Declare(global, new StructTypeSymbol(PredefinedTypeNames.Usz, global));
        var f16 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.F16, global));
        var f32 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.F32, global));
        var f64 = Declare(global, new StructTypeSymbol(PredefinedTypeNames.F64, global));

        // Type is its own type. We need to avoid recursion.
        SetType(type, type);
        SetType(module, type);

        // Global module type `module` is contained within the global module. We need to avoid recursion.
        SetType(global, module);

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
            Kind = SymbolKind.Module,
            Syntax = SyntaxToken.CreateSynthetic(SyntaxKind.ModuleKeyword),
            Name = "<global>",
            Type = null!,
            ContainingSymbol = null!,
            ContainingModule = null!,
            IsStatic = true,
            IsReadOnly = true
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

    private static StructTypeSymbol MakeModuleType(ModuleSymbol global)
    {
        var type = (StructTypeSymbol)RuntimeHelpers.GetUninitializedObject(typeof(StructTypeSymbol)) with
        {
            Kind = SymbolKind.StructType,
            Syntax = SyntaxToken.CreateSynthetic(SyntaxKind.TypeKeyword),
            Name = PredefinedTypeNames.Module,
            Type = null!,
            ContainingSymbol = global,
            ContainingModule = global,
            IsStatic = true,
            IsReadOnly = true,
        };

        GetMembersField(type) = [];

        return type;
    }

    private static StructTypeSymbol MakeTypeType(ModuleSymbol global)
    {
        var type = (StructTypeSymbol)RuntimeHelpers.GetUninitializedObject(typeof(StructTypeSymbol)) with
        {
            Kind = SymbolKind.StructType,
            Syntax = SyntaxToken.CreateSynthetic(SyntaxKind.TypeKeyword),
            Name = PredefinedTypeNames.Type,
            Type = null!,
            ContainingSymbol = global,
            ContainingModule = global,
            IsStatic = true,
            IsReadOnly = true,
        };

        GetMembersField(type) = [];

        return type;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_members")]
    static extern ref Dictionary<NameString, Symbol> GetMembersField(ContainerSymbol container);

    private static T Declare<T>(ContainerSymbol container, T symbol) where T : Symbol
    {
        if (!container.Declare(symbol))
            throw new InvalidOperationException($"Could not declare symbol '{symbol}'");
        symbol.IsIntrinsic = true;
        return symbol;
    }

    private static void DeclareEqualityOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    Name: OperatorKind.Equals.GetName(type, type),
                    Type: type.ContainingModule.BoolType,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.NotEquals.GetName(type, type),
                    type.ContainingModule.BoolType,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));
        }
    }

    private static void DeclareComparisonOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LessThan.GetName(type, type),
                    type.ContainingModule.BoolType,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LessThanOrEqual.GetName(type, type),
                    type.ContainingModule.BoolType,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.GreaterThan.GetName(type, type),
                    type.ContainingModule.BoolType,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.GreaterThanOrEqual.GetName(type, type),
                    type.ContainingModule.BoolType,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));
        }
    }

    private static void DeclareMathOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.UnaryPlus.GetName(type),
                    type,
                    OperandTypes: [type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.UnaryMinus.GetName(type),
                    type,
                    OperandTypes: [type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Addition.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Subtraction.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Multiplication.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Division.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Modulo.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.Exponentiation.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));
        }
    }

    private static void DeclareBitwiseOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.OnesComplement.GetName(type),
                    type,
                    OperandTypes: [type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.BitwiseAnd.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.BitwiseOr.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.ExclusiveOr.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LeftShift.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.RightShift.GetName(type, type),
                    type,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));
        }
    }

    private static void DeclareLogicalOperators(params ReadOnlySpan<StructTypeSymbol> types)
    {
        foreach (var type in types)
        {
            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LogicalNot.GetName(type),
                    type.ContainingModule.BoolType,
                    OperandTypes: [type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LogicalAnd.GetName(type, type),
                    type.ContainingModule.BoolType,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));

            Declare(
                type,
                new OperatorSymbol(
                    OperatorKind.LogicalOr.GetName(type, type),
                    type.ContainingModule.BoolType,
                    OperandTypes: [type, type],
                    ContainingSymbol: type,
                    IsStatic: true,
                    IsReadOnly: true));
        }
    }

    private static void DeclareStringOperators(StructTypeSymbol type)
    {
        Declare(
            type,
            new OperatorSymbol(
                OperatorKind.Addition.GetName(type, type),
                type,
                OperandTypes: [type, type],
                ContainingSymbol: type,
                IsStatic: true,
                IsReadOnly: true));

        Declare(
            type,
            new OperatorSymbol(
                OperatorKind.Addition.GetName(type, type.ContainingModule.AnyType),
                type,
                OperandTypes: [type, type.ContainingModule.AnyType],
                ContainingSymbol: type,
                IsStatic: true,
                IsReadOnly: true));

        Declare(
            type,
            new OperatorSymbol(
                OperatorKind.Addition.GetName(type.ContainingModule.AnyType, type),
                type,
                OperandTypes: [type.ContainingModule.AnyType, type],
                ContainingSymbol: type,
                IsStatic: true,
                IsReadOnly: true));
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
        DeclareConversions(source, isImplicit: true, implicitTargets);
        DeclareConversions(source, isImplicit: false, explicitTargets);
    }

    private static void DeclareConversions(
        StructTypeSymbol source,
        bool isImplicit,
        ReadOnlySpan<StructTypeSymbol> targets)
    {
        foreach (var target in targets)
        {
            Declare(
                source,
                new ConversionSymbol(
                    ConversionSymbol.GetName(source, target),
                    target,
                    source,
                    isImplicit,
                    source.ContainingSymbol));
        }
    }
}
