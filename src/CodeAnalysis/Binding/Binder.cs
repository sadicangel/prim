using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

internal static partial class Binder
{
    private static readonly List<DeclarationSyntax> s_emptyDeclarationList = [];

    public static BoundCompilationUnit Bind(BoundTree boundTree, BoundScope boundScope)
    {
        var context = new BindingContext(boundTree, boundScope);

        var compilationUnit = boundTree.SyntaxTree.Root;

        var declarations = compilationUnit.SyntaxNodes
            .OfType<DeclarationSyntax>()
            .GroupBy(d => d.SyntaxKind)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.StructDeclaration, s_emptyDeclarationList))
            DeclareStruct((StructDeclarationSyntax)declaration, context);

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.FunctionDeclaration, s_emptyDeclarationList))
            DeclareFunction((FunctionDeclarationSyntax)declaration, context);

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.VariableDeclaration, s_emptyDeclarationList))
            DeclareVariable((VariableDeclarationSyntax)declaration, context);

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.StructDeclaration, s_emptyDeclarationList))
            DeclareStructMembers((StructDeclarationSyntax)declaration, context);

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

    private static StructSymbol DeclareStruct(StructDeclarationSyntax syntax, BindingContext context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        var structSymbol = new StructSymbol(syntax, structName, new NamedType(structName));
        if (!context.BoundScope.Declare(structSymbol))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, structName);
        return structSymbol;
    }

    private static void DeclareStructMembers(StructDeclarationSyntax syntax, BindingContext context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(structName) is not StructSymbol structSymbol)
            throw new UnreachableException($"Type '{structName}' was not declared");

        var properties = new List<Property>(syntax.Properties.Count);
        var declaredPropertyNames = new HashSet<string>();
        {
            foreach (var propertySyntax in syntax.Properties)
            {
                var propertyName = propertySyntax.IdentifierToken.Text.ToString();
                var propertyType = BindType(propertySyntax.Type, context);
                if (!declaredPropertyNames.Add(propertyName))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, propertyName);
                var property = new Property(propertyName, propertyType, propertySyntax.IsReadOnly);
                properties.Add(property);
            }
        }

        structSymbol = new StructSymbol(syntax, structName, new NamedType(structName)
        {
            Properties = [.. properties]
        });

        context.BoundScope.Replace(structSymbol);
    }

    private static FunctionSymbol DeclareFunction(FunctionDeclarationSyntax syntax, BindingContext context)
    {
        var functionName = syntax.IdentifierToken.Text.ToString();
        var functionType = (FunctionType)BindType(syntax.Type, context);
        var functionSymbol = new FunctionSymbol(syntax, functionName, functionType);
        if (!context.BoundScope.Declare(functionSymbol))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, functionName);
        return functionSymbol;
    }

    private static VariableSymbol DeclareVariable(VariableDeclarationSyntax syntax, BindingContext context)
    {
        var variableName = syntax.IdentifierToken.Text.ToString();
        var variableType = BindType(syntax.Type, context);
        var variableSymbol = new VariableSymbol(syntax, variableName, variableType, syntax.IsReadOnly);
        if (!context.BoundScope.Declare(variableSymbol))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, variableName);
        return variableSymbol;
    }
}
