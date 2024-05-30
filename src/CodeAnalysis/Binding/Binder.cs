using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

internal static partial class Binder
{
    public static BoundCompilationUnit Bind(BoundTree boundTree, BoundScope boundScope)
    {
        var context = new BindingContext(boundTree, boundScope);

        var compilationUnit = boundTree.SyntaxTree.Root;

        var declarations = compilationUnit.SyntaxNodes
            .OfType<DeclarationSyntax>()
            .GroupBy(d => d.SyntaxKind)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.StructDeclaration, []))
            DeclareStruct((StructDeclarationSyntax)declaration, context);

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.FunctionDeclaration, []))
            DeclareFunction((FunctionDeclarationSyntax)declaration, context);

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.VariableDeclaration, []))
            DeclareVariable((VariableDeclarationSyntax)declaration, context);

        return BindCompilationUnit(compilationUnit, context);
    }

    private static Symbol Declare(DeclarationSyntax syntax, BindingContext context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.VariableDeclaration => DeclareVariable((VariableDeclarationSyntax)syntax, context),
            SyntaxKind.FunctionDeclaration => DeclareFunction((FunctionDeclarationSyntax)syntax, context),
            SyntaxKind.StructDeclaration => DeclareStruct((StructDeclarationSyntax)syntax, context),
            _ => throw new UnreachableException($"Unexpected {nameof(DeclarationSyntax)} '{syntax.GetType().Name}'")
        };
    }

    private static NamedTypeSymbol DeclareStruct(StructDeclarationSyntax syntax, BindingContext context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        var namedTypeSymbol = new NamedTypeSymbol(new StructSymbol(structName));
        if (!context.BoundScope.Declare(namedTypeSymbol))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, structName);
        return namedTypeSymbol;
    }

    private static FunctionTypeSymbol DeclareFunction(FunctionDeclarationSyntax syntax, BindingContext context)
    {
        var functionName = syntax.IdentifierToken.Text.ToString();
        var functionType = new FunctionTypeSymbol(new FunctionSymbol(functionName));
        if (!context.BoundScope.Declare(functionType))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, functionName);
        return functionType;
    }

    private static VariableSymbol DeclareVariable(VariableDeclarationSyntax syntax, BindingContext context)
    {
        var variableName = syntax.IdentifierToken.Text.ToString();
        var variableType = new VariableSymbol(variableName, syntax.IsReadOnly);
        if (!context.BoundScope.Declare(variableType))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, variableName);
        return variableType;
    }
}
