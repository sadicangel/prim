using System.Diagnostics;
using System.Numerics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions.Declarations;

namespace CodeAnalysis.Binding.Symbols;

internal abstract record class TypeSymbol(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    string Name,
    ModuleSymbol ContainingModule)
    : Symbol(BoundKind, Syntax, Name, ContainingModule.RuntimeType, ContainingModule, ContainingModule, IsStatic: true, IsReadOnly: true)
{
    private readonly List<Symbol> _members = [];

    public bool IsAny { get => Name is PredefinedTypes.Any; }
    public bool IsErr { get => Name is PredefinedTypes.Err; }
    public bool IsUnit { get => Name is PredefinedTypes.Unit; }
    public bool IsArray { get => this is ArrayTypeSymbol; }
    public bool IsLambda { get => this is LambdaTypeSymbol; }
    public abstract bool IsNever { get; }
    public bool IsOption { get => this is OptionTypeSymbol; }
    public bool IsUnion { get => this is UnionTypeSymbol; }
    public bool IsUnknown { get => Name is PredefinedTypes.Unknown; }
    public bool IsPredefined { get => PredefinedTypes.All.Contains(Name); }

    public override IEnumerable<Symbol> DeclaredSymbols => [];

    public virtual bool Equals(TypeSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();

    public Type GetCrlType() => Name switch
    {
        "unit" => typeof(Unit),
        "str" => typeof(string),
        "bool" => typeof(bool),
        "i8" => typeof(sbyte),
        "i16" => typeof(short),
        "i32" => typeof(int),
        "i64" => typeof(long),
        "i128" => typeof(BigInteger),
        "isz" => typeof(nint),
        "u8" => typeof(byte),
        "u16" => typeof(ushort),
        "u32" => typeof(uint),
        "u64" => typeof(ulong),
        "u128" => typeof(BigInteger),
        "usz" => typeof(nuint),
        "f16" => typeof(Half),
        "f32" => typeof(float),
        "f64" => typeof(double),
        "f80" => typeof(double),
        "f128" => typeof(double),
        _ => throw new UnreachableException($"Unexpected built-in {nameof(TypeSymbol)} '{Name}'"),
    };

    internal bool IsCoercibleFrom(TypeSymbol type) => IsConvertibleFrom(type, out _);
    internal bool IsCoercibleFrom(TypeSymbol type, out ConversionSymbol? conversion) =>
        IsConvertibleFrom(type, out conversion) && conversion?.IsExplicit is not true;

    internal bool IsCoercibleTo(TypeSymbol type) => IsCoercibleTo(type, out _);
    internal bool IsCoercibleTo(TypeSymbol type, out ConversionSymbol? conversion) => type.IsCoercibleFrom(this, out conversion);

    internal bool IsConvertibleFrom(TypeSymbol type) => IsConvertibleFrom(type, out _);
    internal abstract bool IsConvertibleFrom(TypeSymbol type, out ConversionSymbol? conversion);

    internal bool IsConvertibleTo(TypeSymbol type) => IsConvertibleTo(type, out _);
    internal bool IsConvertibleTo(TypeSymbol type, out ConversionSymbol? conversion) => type.IsConvertibleFrom(this, out conversion);

    internal List<Symbol> GetSymbols(string name)
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
        AddProperty(name, type, isStatic: false, isReadOnly: false, syntax);

    internal bool AddProperty(string name, TypeSymbol type, bool isStatic, bool isReadOnly, SyntaxNode? syntax = null)
    {
        if (GetProperty(name) is not null) return false;

        // TODO: Allow static properties.
        var propertySymbol = new PropertySymbol(
            syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            name,
            type,
            ContainingType: this,
            isStatic,
            isReadOnly);
        _members.Add(propertySymbol);

        return true;
    }

    internal PropertySymbol? GetProperty(string name)
    {
        foreach (var property in _members.OfType<PropertySymbol>())
        {
            if (name.Equals(property.Name, StringComparison.Ordinal))
                return property;
        }
        return null;
    }

    internal bool AddMethod(string nameNoMangling, LambdaTypeSymbol type, MethodDeclarationSyntax syntax) =>
        AddMethod(nameNoMangling, type, isStatic: true, isReadOnly: true, syntax);

    internal bool AddMethod(string nameNoMangling, LambdaTypeSymbol type, bool isStatic = true, bool isReadOnly = true, SyntaxNode? syntax = null)
    {
        if (GetProperty(nameNoMangling) is not null) return false;
        if (GetMethod(nameNoMangling, type) is not null) return false;

        var methodSymbol = new MethodSymbol(
            syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            nameNoMangling,
            type,
            ContainingType: this,
            isStatic,
            isReadOnly);
        _members.Add(methodSymbol);

        return true;
    }

    internal MethodSymbol? GetMethod(ReadOnlySpan<char> nameNoMangling, LambdaTypeSymbol type)
    {
        foreach (var method in _members.OfType<MethodSymbol>())
        {
            if (nameNoMangling.Equals(method.NameNoMangling, StringComparison.Ordinal) && type == method.LambdaType)
                return method;
        }
        return null;
    }

    internal MethodSymbol? GetMethod(ReadOnlySpan<char> name)
    {
        foreach (var method in _members.OfType<MethodSymbol>())
        {
            if (name.Equals(method.Name, StringComparison.Ordinal))
                return method;
        }
        return null;
    }

    internal bool AddOperator(LambdaTypeSymbol type, OperatorDeclarationSyntax syntax) =>
        AddOperator(syntax.OperatorToken.SyntaxKind, type, syntax);

    internal bool AddOperator(SyntaxKind operatorKind, LambdaTypeSymbol type, SyntaxNode? syntax = null)
    {
        if (GetOperator(operatorKind, type) is not null) return false;

        var methodSymbol = new OperatorSymbol(
            operatorKind,
            syntax ?? SyntaxFactory.SyntheticToken(operatorKind),
            type,
            ContainingType: this);
        _members.Add(methodSymbol);

        return true;
    }

    internal OperatorSymbol? GetOperator(SyntaxKind operatorKind, LambdaTypeSymbol type) =>
        _members.OfType<OperatorSymbol>().SingleOrDefault(o => o.OperatorKind == operatorKind && o.LambdaType == type);

    internal List<OperatorSymbol> GetOperators(SyntaxKind operatorKind, Func<LambdaTypeSymbol, bool>? filter = null)
    {
        var operators = _members.OfType<OperatorSymbol>()
            .Where(o => o.OperatorKind == operatorKind);

        if (filter is not null)
            operators = operators.Where(o => filter(o.LambdaType));

        return operators.ToList();
    }

    internal List<OperatorSymbol> GetUnaryOperators(SyntaxKind operatorKind, TypeSymbol operandType, TypeSymbol? resultType = null)
    {
        var operators = _members.OfType<OperatorSymbol>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.IsUnaryOperator)
            .Where(o => operandType.IsCoercibleTo(o.Parameters[0].Type))
            .Where(o => resultType is null || resultType.IsCoercibleTo(o.ReturnType))
            .ToList();

        if (operators.Count > 1)
        {
            var @operator = operators.SingleOrDefault(o =>
                o.OperatorKind == operatorKind &&
                o.IsUnaryOperator &&
                o.Parameters[0].Type == operandType);

            if (@operator is not null)
                return [@operator];
        }

        return operators;
    }

    internal List<OperatorSymbol> GetBinaryOperators(SyntaxKind operatorKind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol? resultType = null)
    {
        var operators = _members.OfType<OperatorSymbol>()
            .Where(o => o.OperatorKind == operatorKind)
            .Where(o => o.IsBinaryOperator)
            .Where(o => leftType.IsCoercibleTo(o.Parameters[0].Type))
            .Where(o => rightType.IsCoercibleTo(o.Parameters[1].Type))
            .Where(o => resultType is null || resultType.IsCoercibleTo(o.ReturnType))
            .ToList();

        if (operators.Count > 1)
        {
            var @operator = operators.SingleOrDefault(o =>
                o.OperatorKind == operatorKind &&
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
            type,
            ContainingType: this);
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
