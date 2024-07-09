using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionSymbol(
    SyntaxNode Syntax,
    string Name,
    FunctionType FunctionType,
    BoundList<VariableSymbol> Parameters,
    bool IsReadOnly,
    bool IsStatic)
    : Symbol(BoundKind.FunctionSymbol, Syntax, Name, FunctionType, IsReadOnly, IsStatic)
{
    public PrimType ReturnType { get => FunctionType.ReturnType; }

    public static FunctionSymbol FromConversion(
        Conversion conversion,
        ConversionDeclarationSyntax? syntax = null)
    {
        var parameter = conversion.Type.Parameters[0];
        var parameterSymbol = new VariableSymbol(
            syntax?.Type.Parameters[0] as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            parameter.Name,
            parameter.Type,
            IsReadOnly: false);

        return new FunctionSymbol(
            syntax as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            conversion.Name,
            conversion.Type,
            [parameterSymbol],
            IsReadOnly: true,
            IsStatic: true);
    }

    public static FunctionSymbol FromMethod(
        Method method,
        PrimType containingType,
        MethodDeclarationSyntax? syntax = null)
    {
        var parameters = new BoundList<VariableSymbol>.Builder(method.Type.Parameters.Count + 1);
        if (!method.IsStatic)
        {
            parameters.Add(VariableSymbol.This(containingType));
        }
        foreach (var parameter in method.Type.Parameters.Select((p, i) =>
            new VariableSymbol(
                syntax?.Type.Parameters[i] as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
                p.Name,
                p.Type,
                IsReadOnly: true)))
        {
            parameters.Add(parameter);
        }

        // TODO: Allow not having to use `this`?
        var methodSymbol = new FunctionSymbol(
            syntax as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            method.Name,
            method.Type,
            parameters.ToBoundList(),
            IsReadOnly: method.IsReadOnly,
            IsStatic: method.IsStatic);

        return methodSymbol;
    }

    public static FunctionSymbol FromOperator(Operator @operator, OperatorDeclarationSyntax? syntax = null)
    {
        var parameters = @operator.Type.Parameters.Select((p, i) => new VariableSymbol(
            syntax?.Type.Parameters[i] as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            p.Name,
            p.Type,
            IsReadOnly: true));

        return new FunctionSymbol(
            syntax as SyntaxNode ?? SyntaxFactory.SyntheticToken(@operator.OperatorKind),
            @operator.Name,
            @operator.Type,
            [.. parameters],
            IsReadOnly: @operator.IsReadOnly,
            IsStatic: @operator.IsStatic);
    }
}
