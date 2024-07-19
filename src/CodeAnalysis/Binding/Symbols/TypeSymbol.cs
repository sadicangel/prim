using System.Diagnostics;
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

    public bool IsAny { get => this == PredefinedTypes.Any; }
    public bool IsArray { get => this is ArrayTypeSymbol; }
    public bool IsLambda { get => this is LambdaTypeSymbol; }
    public abstract bool IsNever { get; }
    public bool IsOption { get => this is OptionTypeSymbol; }
    public bool IsUnion { get => this is UnionTypeSymbol; }
    public bool IsUnknown { get => this == PredefinedTypes.Unknown; }
    public bool IsPredefined { get => PredefinedTypeNames.All.Contains(Name); }

    public virtual bool Equals(TypeSymbol? other) => base.Equals(other);

    public override int GetHashCode() => base.GetHashCode();

    internal static string GetMethodName(ReadOnlySpan<char> name, LambdaTypeSymbol type) =>
        $"{name}<{string.Join(',', type.Parameters.Select(p => p.Type.Name))}>";

    internal static string GetMethodName(SyntaxKind syntaxKind, LambdaTypeSymbol type) =>
        GetMethodName(SyntaxFacts.GetText(syntaxKind) ?? throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntaxKind}'"), type);

    internal bool IsCoercibleTo(TypeSymbol type) => IsCoercibleTo(type, out _);
    internal bool IsCoercibleTo(TypeSymbol type, out MethodSymbol? conversion)
    {
        conversion = null;
        if (type.IsAny || this == type)
        {
            return true;
        }

        conversion = GetConversion(this, type) ?? type.GetConversion(this, type);

        return conversion is not null && conversion.IsImplicitConversion;
    }

    internal bool IsConvertibleTo(TypeSymbol type) => IsConvertibleTo(type, out _);
    internal bool IsConvertibleTo(TypeSymbol type, out MethodSymbol? conversion)
    {
        conversion = null;
        if (type.IsAny || this == type)
        {
            return true;
        }

        conversion = GetConversion(this, type) ?? type.GetConversion(this, type);

        return conversion is not null;
    }

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
        AddProperty(name, type, syntax.IsReadOnly, isStatic: false, syntax);

    internal bool AddProperty(string name, TypeSymbol type, bool isReadOnly, bool isStatic, SyntaxNode? syntax = null)
    {
        if (GetProperty(name) is not null) return false;

        // TODO: Allow static properties.
        var propertySymbol = new PropertySymbol(
            syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            name,
            type,
            ContainingSymbol: this,
            isReadOnly,
            isStatic);
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
        AddMethod(name, type, syntax.IsReadOnly, isStatic: true, syntax);

    internal bool AddMethod(string name, LambdaTypeSymbol type, bool isReadOnly = true, bool isStatic = true, SyntaxNode? syntax = null)
    {
        if (GetMethod(name, type) is not null) return false;

        // TODO: Allow static methods.
        // TODO: Allow not having to use `this`?
        type = type with
        {
            Parameters = [.. type.Parameters.Prepend(VariableSymbol.This(this))]
        };

        var methodSymbol = new MethodSymbol(
            SyntaxKind.IdentifierToken,
            syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            GetMethodName(name, type),
            type,
            ContainingSymbol: this,
            isReadOnly,
            isStatic);
        _members.Add(methodSymbol);

        return true;
    }

    internal MethodSymbol? GetMethod(ReadOnlySpan<char> name, LambdaTypeSymbol type)
        => GetMethod(GetMethodName(name, type));

    internal MethodSymbol? GetMethod(string name) =>
        _members.OfType<MethodSymbol>().SingleOrDefault(m => m.Name == name);

    internal bool AddOperator(LambdaTypeSymbol type, OperatorDeclarationSyntax syntax) =>
        AddOperator(syntax.OperatorToken.SyntaxKind, type, syntax.IsReadOnly, isStatic: true, syntax);

    internal bool AddOperator(SyntaxKind operatorKind, LambdaTypeSymbol type, bool isReadOnly = true, bool isStatic = true, SyntaxNode? syntax = null)
    {
        if (GetOperator(operatorKind, type) is not null) return false;

        var methodSymbol = new MethodSymbol(
            operatorKind,
            syntax ?? SyntaxFactory.SyntheticToken(operatorKind),
            GetMethodName(operatorKind, type),
            type,
            ContainingSymbol: this,
            isReadOnly,
            isStatic);
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
        AddOperator(syntax.ConversionKeyword.SyntaxKind, type, syntax.IsReadOnly, isStatic: true, syntax);

    internal bool AddConversion(SyntaxKind conversionKind, LambdaTypeSymbol type, bool isReadOnly = true, bool isStatic = true, SyntaxNode? syntax = null)
    {
        if (GetConversion(type) is not null) return false;

        var methodSymbol = new MethodSymbol(
            conversionKind,
            syntax ?? SyntaxFactory.SyntheticToken(conversionKind),
            GetMethodName(conversionKind, type),
            type,
            ContainingSymbol: this,
            isReadOnly,
            isStatic);
        _members.Add(methodSymbol);

        return true;
    }

    internal MethodSymbol? GetConversion(LambdaTypeSymbol type)
    {
        return _members.OfType<MethodSymbol>()
            .Where(x => x.Type == type)
            .SingleOrDefault();
    }

    internal MethodSymbol? GetConversion(TypeSymbol sourceType, TypeSymbol targetType)
    {
        return _members.OfType<MethodSymbol>()
            .Where(x => x.IsConversion)
            .Where(x => x.Parameters[0].Type.IsAny || x.Parameters[0].Type == sourceType)
            .Where(x => x.ReturnType.IsAny || x.ReturnType == targetType)
            .SingleOrDefault();
    }
}
