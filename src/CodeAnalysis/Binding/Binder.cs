using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

internal static class Binder
{
    public static BoundCompilationUnit Bind(BoundTree boundTree, BoundScope boundScope)
    {
        var context = new BindingContext(boundTree, boundScope);

        var compilationUnit = boundTree.SyntaxTree.Root;
        // Bind declarations.
        var declarations = compilationUnit.SyntaxNodes.OfType<DeclarationSyntax>().OrderByDescending(n => n.SyntaxKind);
        foreach (var declaration in declarations)
        {
            switch (declaration.SyntaxKind)
            {
                case SyntaxKind.StructDeclaration:
                    DeclareStruct((StructDeclarationSyntax)declaration, context);
                    break;
                case SyntaxKind.FunctionDeclaration:
                    DeclareFunction((FunctionDeclarationSyntax)declaration, context);
                    break;
                case SyntaxKind.VariableDeclaration:
                    DeclareVariable((VariableDeclarationSyntax)declaration, context);
                    break;
                default:
                    throw new UnreachableException($"Unexpected declaration '{declaration.SyntaxKind}'");
            };

            static Symbol DeclareStruct(StructDeclarationSyntax structDeclaration, BindingContext context)
            {
                var structName = structDeclaration.IdentifierToken.Text.ToString();
                var namedTypeSymbol = new NamedTypeSymbol(new StructSymbol(structName));
                if (!context.BoundScope.Declare(namedTypeSymbol))
                    context.Diagnostics.ReportSymbolRedeclaration(structDeclaration.Location, structName);
                return namedTypeSymbol;
            }

            static Symbol DeclareFunction(FunctionDeclarationSyntax functionDeclaration, BindingContext context)
            {
                var functionName = functionDeclaration.IdentifierToken.Text.ToString();
                var functionType = new FunctionTypeSymbol(new FunctionSymbol(functionName));
                if (!context.BoundScope.Declare(functionType))
                    context.Diagnostics.ReportSymbolRedeclaration(functionDeclaration.Location, functionName);
                return functionType;
            }

            static Symbol DeclareVariable(VariableDeclarationSyntax variableDeclaration, BindingContext context)
            {
                var variableName = variableDeclaration.IdentifierToken.Text.ToString();
                var variableType = new VariableSymbol(variableName, variableDeclaration.IsReadOnly);
                if (!context.BoundScope.Declare(variableType))
                    context.Diagnostics.ReportSymbolRedeclaration(variableDeclaration.Location, variableName);
                return variableType;
            }
        }

        return new BoundCompilationUnit(compilationUnit, []);
    }
}
