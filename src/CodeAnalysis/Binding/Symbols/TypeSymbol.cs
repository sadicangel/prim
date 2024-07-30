﻿using System.Diagnostics;
using System.Numerics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding.Symbols;

internal abstract record class TypeSymbol(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type)
    : Symbol(BoundKind, Syntax, Name, Type, IsStatic: true, IsReadOnly: true)
{
    private readonly List<Symbol> _members = [];

    public bool IsAny { get => this == PredefinedSymbols.Any; }
    public bool IsErr { get => this == PredefinedSymbols.Err; }
    public bool IsUnit { get => this == PredefinedSymbols.Unit; }
    public bool IsArray { get => this is ArrayTypeSymbol; }
    public bool IsLambda { get => this is LambdaTypeSymbol; }
    public abstract bool IsNever { get; }
    public bool IsOption { get => this is OptionTypeSymbol; }
    public bool IsUnion { get => this is UnionTypeSymbol; }
    public bool IsUnknown { get => this == PredefinedSymbols.Unknown; }
    public bool IsPredefined { get => PredefinedSymbolNames.All.Contains(Name); }

    public virtual IEnumerable<Symbol> DeclaredSymbols { get; } = [];

    public virtual bool Equals(TypeSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();

    public Type GetCrlType() => Name switch
    {
        PredefinedSymbolNames.Unit => typeof(Unit),
        PredefinedSymbolNames.Str => typeof(string),
        PredefinedSymbolNames.Bool => typeof(bool),
        PredefinedSymbolNames.I8 => typeof(sbyte),
        PredefinedSymbolNames.I16 => typeof(short),
        PredefinedSymbolNames.I32 => typeof(int),
        PredefinedSymbolNames.I64 => typeof(long),
        PredefinedSymbolNames.I128 => typeof(BigInteger),
        PredefinedSymbolNames.ISize => typeof(nint),
        PredefinedSymbolNames.U8 => typeof(byte),
        PredefinedSymbolNames.U16 => typeof(ushort),
        PredefinedSymbolNames.U32 => typeof(uint),
        PredefinedSymbolNames.U64 => typeof(ulong),
        PredefinedSymbolNames.U128 => typeof(BigInteger),
        PredefinedSymbolNames.USize => typeof(nuint),
        PredefinedSymbolNames.F16 => typeof(Half),
        PredefinedSymbolNames.F32 => typeof(float),
        PredefinedSymbolNames.F64 => typeof(double),
        PredefinedSymbolNames.F80 => typeof(double),
        PredefinedSymbolNames.F128 => typeof(double),
        _ => throw new UnreachableException($"Unexpected built-in {nameof(TypeSymbol)} '{Name}'"),
    };

    public static TypeSymbol FromSet(HashSet<TypeSymbol> types, SyntaxNode? syntax = null) => types switch
    {
        { Count: 0 } => PredefinedSymbols.Unknown,
        { Count: 1 } => types.Single(),
        _ when types.Contains(PredefinedSymbols.Never) => PredefinedSymbols.Never,
        _ => new UnionTypeSymbol(syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.UnionType), [.. types]),
    };

    internal static string GetMethodName(ReadOnlySpan<char> name, LambdaTypeSymbol type) =>
        $"{name}<{string.Join(',', type.Parameters.Select(p => p.Type.Name))}>";

    internal static string GetMethodName(SyntaxKind syntaxKind, LambdaTypeSymbol type) =>
        GetMethodName(SyntaxFacts.GetText(syntaxKind) ?? throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntaxKind}'"), type);

    internal bool IsCoercibleFrom(TypeSymbol type) => IsConvertibleFrom(type, out _);
    internal bool IsCoercibleFrom(TypeSymbol type, out ConversionSymbol? conversion) =>
        IsConvertibleFrom(type, out conversion) && conversion?.IsExplicit is not true;

    internal bool IsCoercibleTo(TypeSymbol type) => IsCoercibleTo(type, out _);
    internal bool IsCoercibleTo(TypeSymbol type, out ConversionSymbol? conversion) => type.IsCoercibleFrom(this, out conversion);

    internal bool IsConvertibleFrom(TypeSymbol type) => IsConvertibleFrom(type, out _);
    internal abstract bool IsConvertibleFrom(TypeSymbol type, out ConversionSymbol? conversion);

    internal bool IsConvertibleTo(TypeSymbol type) => IsConvertibleTo(type, out _);
    internal bool IsConvertibleTo(TypeSymbol type, out ConversionSymbol? conversion) => type.IsConvertibleFrom(this, out conversion);

    internal List<Symbol> GetSymbols(ReadOnlySpan<char> name)
    {
        var list = new List<Symbol>();
        foreach (var member in _members)
        {
            var index = member.Name.IndexOf('<');
            var memberName = index >= 0 ? member.Name.AsSpan(0, index) : member.Name;
            if (memberName.Equals(name, StringComparison.Ordinal))
                list.Add(member);
        }
        return list;
    }

    internal bool AddProperty(string name, TypeSymbol type, PropertyDeclarationSyntax syntax) =>
        AddProperty(name, type, isStatic: false, syntax.IsReadOnly, syntax);

    internal bool AddProperty(string name, TypeSymbol type, bool isStatic, bool isReadOnly, SyntaxNode? syntax = null)
    {
        if (GetProperty(name) is not null) return false;

        // TODO: Allow static properties.
        var propertySymbol = new PropertySymbol(
            syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            name,
            type,
            ContainingSymbol: this,
            isStatic,
            isReadOnly);
        _members.Add(propertySymbol);

        return true;
    }

    internal PropertySymbol? GetProperty(ReadOnlySpan<char> name)
    {
        foreach (var property in _members.OfType<PropertySymbol>())
        {
            if (name.Equals(property.Name, StringComparison.Ordinal))
                return property;
        }
        return null;
    }

    internal bool AddMethod(string name, LambdaTypeSymbol type, MethodDeclarationSyntax syntax) =>
        AddMethod(name, type, isStatic: true, syntax.IsReadOnly, syntax);

    internal bool AddMethod(string name, LambdaTypeSymbol type, bool isStatic = true, bool isReadOnly = true, SyntaxNode? syntax = null)
    {
        if (GetProperty(name) is not null) return false;
        if (GetMethod(name, type) is not null) return false;

        var methodSymbol = new MethodSymbol(
            SyntaxKind.IdentifierToken,
            syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            GetMethodName(name, type),
            type,
            ContainingSymbol: this,
            isStatic,
            isReadOnly);
        _members.Add(methodSymbol);

        return true;
    }

    internal MethodSymbol? GetMethod(ReadOnlySpan<char> name, LambdaTypeSymbol type)
        => GetMethod(GetMethodName(name, type));

    internal MethodSymbol? GetMethod(string name) =>
        _members.OfType<MethodSymbol>().SingleOrDefault(m => m.Name == name);

    internal bool AddOperator(LambdaTypeSymbol type, OperatorDeclarationSyntax syntax) =>
        AddOperator(syntax.OperatorToken.SyntaxKind, type, isStatic: true, syntax.IsReadOnly, syntax);

    internal bool AddOperator(SyntaxKind operatorKind, LambdaTypeSymbol type, bool isStatic = true, bool isReadOnly = true, SyntaxNode? syntax = null)
    {
        if (GetOperator(operatorKind, type) is not null) return false;

        var methodSymbol = new MethodSymbol(
            operatorKind,
            syntax ?? SyntaxFactory.SyntheticToken(operatorKind),
            GetMethodName(operatorKind, type),
            type,
            ContainingSymbol: this,
            isStatic,
            isReadOnly);
        _members.Add(methodSymbol);

        return true;
    }

    internal MethodSymbol? GetOperator(SyntaxKind operatorKind, LambdaTypeSymbol type)
    {
        foreach (var @operator in _members.OfType<MethodSymbol>())
        {
            if (@operator.MethodKind == operatorKind && @operator.LambdaType == type)
                return @operator;
        }
        return null;
    }

    internal List<MethodSymbol> GetOperators(SyntaxKind operatorKind, Func<LambdaTypeSymbol, bool>? filter = null)
    {
        var operators = _members.OfType<MethodSymbol>()
            .Where(o => o.MethodKind == operatorKind);

        if (filter is not null)
            operators = operators.Where(o => filter(o.LambdaType));

        return operators.ToList();
    }

    internal List<MethodSymbol> GetUnaryOperators(SyntaxKind operatorKind, TypeSymbol operandType, TypeSymbol? resultType = null)
    {
        var operators = _members.OfType<MethodSymbol>()
            .Where(o => o.MethodKind == operatorKind)
            .Where(o => o.IsUnaryOperator)
            .Where(o => operandType.IsCoercibleTo(o.Parameters[0].Type))
            .Where(o => resultType is null || resultType.IsCoercibleTo(o.ReturnType))
            .ToList();

        if (operators.Count > 1)
        {
            var @operator = operators.SingleOrDefault(o =>
                o.MethodKind == operatorKind &&
                o.IsUnaryOperator &&
                o.Parameters[0].Type == operandType);

            if (@operator is not null)
                return [@operator];
        }

        return operators;
    }

    internal List<MethodSymbol> GetBinaryOperators(SyntaxKind operatorKind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol? resultType = null)
    {
        var operators = _members.OfType<MethodSymbol>()
            .Where(o => o.MethodKind == operatorKind)
            .Where(o => o.IsBinaryOperator)
            .Where(o => leftType.IsCoercibleTo(o.Parameters[0].Type))
            .Where(o => rightType.IsCoercibleTo(o.Parameters[1].Type))
            .Where(o => resultType is null || resultType.IsCoercibleTo(o.ReturnType))
            .ToList();

        if (operators.Count > 1)
        {
            var @operator = operators.SingleOrDefault(o =>
                o.MethodKind == operatorKind &&
                o.IsBinaryOperator &&
                o.Parameters[0].Type == leftType &&
                o.Parameters[1].Type == rightType);

            if (@operator is not null)
                return [@operator];
        }

        return operators;
    }

    internal bool AddConversion(LambdaTypeSymbol type, ConversionDeclarationSyntax syntax) =>
        AddConversion(syntax.ConversionKeyword.SyntaxKind, type, syntax);

    internal bool AddConversion(SyntaxKind conversionKind, LambdaTypeSymbol type, SyntaxNode? syntax = null)
    {
        if (GetConversion(type) is not null) return false;

        var conversionSymbol = new ConversionSymbol(
            conversionKind,
            syntax ?? SyntaxFactory.SyntheticToken(conversionKind),
            $"{SyntaxFacts.GetText(conversionKind)}-{type.ReturnType.Name}<{type.Parameters[0].Type.Name}>",
            type,
            ContainingSymbol: this);
        _members.Add(conversionSymbol);

        return true;
    }

    internal ConversionSymbol? GetConversion(LambdaTypeSymbol type)
    {
        return _members.OfType<ConversionSymbol>()
            .Where(x => x.Type == type)
            .SingleOrDefault();
    }

    internal ConversionSymbol? GetConversion(TypeSymbol sourceType, TypeSymbol targetType)
    {
        return _members.OfType<ConversionSymbol>()
            .Where(x => x.Parameter.Type.IsAny || x.Parameter.Type == sourceType)
            .Where(x => x.ReturnType == targetType)
            .SingleOrDefault();
    }
}
