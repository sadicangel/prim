using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class MethodSymbol(
    SyntaxNode Syntax,
    string Name,
    FunctionType FunctionType,
    Symbol? ContainingSymbol,
    bool IsReadOnly,
    bool IsStatic,
    BoundList<VariableSymbol> Parameters)
    : Symbol(
        BoundKind.MethodSymbol,
        Syntax,
        Name,
        FunctionType,
        ContainingSymbol,
        IsReadOnly,
        IsStatic)
{
    public PrimType ReturnType { get => FunctionType.ReturnType; }

    public static MethodSymbol FromConversion(
        Conversion conversion,
        Symbol containingSymbol,
        ConversionDeclarationSyntax? syntax = null)
    {
        var methodSymbol = new MethodSymbol(
            syntax as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            conversion.Name,
            conversion.Type,
            containingSymbol,
            IsReadOnly: true,
            IsStatic: true,
            []);


        var parameter = conversion.Type.Parameters[0];
        var parameterSymbol = new VariableSymbol(
            syntax?.Type.Parameters[0] as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            parameter.Name,
            parameter.Type,
            methodSymbol,
            IsReadOnly: false);

        return methodSymbol with { Parameters = [parameterSymbol] };
    }

    public static MethodSymbol FromMethod(
        Method method,
        Symbol containingSymbol,
        MethodDeclarationSyntax? syntax = null)
    {
        // TODO: Allow not having to use `this`?
        var methodSymbol = new MethodSymbol(
            syntax as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            method.Name,
            method.Type,
            containingSymbol,
            method.IsReadOnly,
            method.IsStatic,
            []);

        var parameters = new BoundList<VariableSymbol>.Builder(method.Type.Parameters.Count + 1);
        if (!method.IsStatic)
        {
            parameters.Add(VariableSymbol.This(containingSymbol.Type, methodSymbol));
        }
        foreach (var parameter in method.Type.Parameters.Select((p, i) =>
            new VariableSymbol(
                syntax?.Type.Parameters[i] as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
                p.Name,
                p.Type,
                methodSymbol,
                IsReadOnly: true)))
        {
            parameters.Add(parameter);
        }

        return methodSymbol with { Parameters = parameters.ToBoundList() };
    }

    public static MethodSymbol FromOperator(
        Operator @operator,
        Symbol containingSymbol,
        OperatorDeclarationSyntax? syntax = null)
    {
        var methodSymbol = new MethodSymbol(
            syntax as SyntaxNode ?? SyntaxFactory.SyntheticToken(@operator.OperatorKind),
            @operator.Name,
            @operator.Type,
            containingSymbol,
            @operator.IsReadOnly,
            @operator.IsStatic,
            []);

        var parameters = @operator.Type.Parameters.Select((p, i) => new VariableSymbol(
            syntax?.Type.Parameters[i] as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            p.Name,
            p.Type,
            methodSymbol,
            IsReadOnly: true));

        return methodSymbol with { Parameters = [.. parameters] };
    }
}
