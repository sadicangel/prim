using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class LambdaTypeSymbol : TypeSymbol
{
    public LambdaTypeSymbol(SyntaxNode syntax, IEnumerable<Parameter> parameters, TypeSymbol returnType)
        : base(
            BoundKind.LambdaTypeSymbol,
            syntax,
            $"({string.Join(", ", parameters.Select(p => p.ToString()))}) -> {returnType.Name}",
            PredefinedTypes.Type)
    {
        Parameters = [.. parameters.Select(p => new VariableSymbol(p.Syntax, p.Name, p.Type, IsReadOnly: false))];
        ReturnType = returnType;
        AddOperator(SyntaxKind.ParenthesisOpenParenthesisCloseToken, this);
    }

    public LambdaTypeSymbol(IEnumerable<Parameter> parameters, TypeSymbol returnType)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.LambdaType), parameters, returnType)
    {
    }

    public BoundList<VariableSymbol> Parameters { get; init; }
    public TypeSymbol ReturnType { get; init; }

    public override bool IsNever => ReturnType.IsNever || Parameters.Any(p => p.Type.IsNever);
}

internal readonly record struct Parameter(SyntaxNode Syntax, string Name, TypeSymbol Type)
{
    public readonly override string ToString() => $"{Name}: {Type.Name}";

    public Parameter(string name, TypeSymbol type)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken), name, type)
    {
    }
}
