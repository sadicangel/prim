using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class LambdaTypeSymbol : TypeSymbol
{
    public LambdaTypeSymbol(SyntaxNode syntax, IEnumerable<Parameter> parameters, TypeSymbol returnType, TypeSymbol runtimeType, ModuleSymbol containingModule)
        : base(
            BoundKind.LambdaTypeSymbol,
            syntax,
            $"({string.Join(", ", parameters.Select(p => p.ToString()))}) -> {returnType.Name}",
            runtimeType,
            containingModule)
    {
        Parameters = [.. parameters.Select(p => new VariableSymbol(p.Syntax, p.Name, p.Type, containingModule, IsStatic: false, IsReadOnly: false))];
        ReturnType = returnType;
        AddOperator(SyntaxKind.ParenthesisOpenParenthesisCloseToken, this);
    }

    public LambdaTypeSymbol(IEnumerable<Parameter> parameters, TypeSymbol returnType, TypeSymbol runtimeType, ModuleSymbol containingModule)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.LambdaType), parameters, returnType, runtimeType, containingModule)
    {
    }

    public BoundList<VariableSymbol> Parameters { get; init; }
    public TypeSymbol ReturnType { get; init; }
    public override IEnumerable<Symbol> DeclaredSymbols => Parameters;

    public override bool IsNever => ReturnType.IsNever || Parameters.Any(p => p.Type.IsNever);

    internal override bool IsConvertibleFrom(TypeSymbol type, out ConversionSymbol? conversion)
    {
        // Because we allow lambdas without parameters, we need to be able to coerce
        // any non lambda expression to a lambda return type, instead of its full type.
        if (type is not LambdaTypeSymbol)
        {
            return ReturnType.IsConvertibleFrom(type, out conversion);
        }

        conversion = null;
        if (type == this)
        {
            return true;
        }

        conversion = GetConversion(type, this) ?? type.GetConversion(type, this);

        return conversion is not null;
    }
}

internal readonly record struct Parameter(SyntaxNode Syntax, string Name, TypeSymbol Type)
{
    public readonly override string ToString() => $"{Name}: {Type.Name}";

    public Parameter(string name, TypeSymbol type)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, type)
    {
    }
}
