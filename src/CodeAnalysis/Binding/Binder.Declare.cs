using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static Symbol Declare(DeclarationSyntax syntax, BinderContext context, bool isForwardDeclarationOnly = false)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.VariableDeclaration => DeclareVariable((VariableDeclarationSyntax)syntax, context),
            SyntaxKind.FunctionDeclaration => DeclareFunction((FunctionDeclarationSyntax)syntax, context),
            SyntaxKind.StructDeclaration => DeclareStruct((StructDeclarationSyntax)syntax, context, isForwardDeclarationOnly),
            _ => throw new UnreachableException($"Unexpected {nameof(DeclarationSyntax)} '{syntax.GetType().Name}'")
        };
    }

    private static StructSymbol DeclareStruct(StructDeclarationSyntax syntax, BinderContext context, bool isForwardDeclarationOnly = false)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        if (isForwardDeclarationOnly)
        {
            var structSymbol = new StructSymbol(syntax, new StructType(structName));
            if (!context.BoundScope.Declare(structSymbol))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, structName);
            return structSymbol;
        }
        else
        {
            if (context.BoundScope.Lookup(structName) is not StructSymbol structSymbol)
            {
                structSymbol = new StructSymbol(syntax, new StructType(structName));
                if (!context.BoundScope.Declare(structSymbol))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, structName);
            }

            foreach (var memberSyntax in syntax.Members)
            {
                _ = memberSyntax.SyntaxKind switch
                {
                    SyntaxKind.PropertyDeclaration => BindProperty((PropertyDeclarationSyntax)memberSyntax, structSymbol, context),
                    SyntaxKind.MethodDeclaration => BindMethod((MethodDeclarationSyntax)memberSyntax, structSymbol, context),
                    SyntaxKind.OperatorDeclaration => BindOperator((OperatorDeclarationSyntax)memberSyntax, structSymbol, context),
                    SyntaxKind.ConversionDeclaration => BindConversion((ConversionDeclarationSyntax)memberSyntax, structSymbol, context),
                    _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{memberSyntax.SyntaxKind}'")
                };
            }

            return structSymbol;


            static int BindProperty(PropertyDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
            {
                var name = syntax.IdentifierToken.Text.ToString();
                var type = BindType(syntax.Type, context);
                if (!structSymbol.Type.AddProperty(name, type, syntax.IsReadOnly))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, name);

                return 0;
            }

            static int BindMethod(MethodDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
            {
                var name = syntax.IdentifierToken.Text.ToString();
                var type = (FunctionType)BindType(syntax.Type, context);
                if (!structSymbol.Type.AddMethod(name, type))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, name);
                return 0;
            }

            static int BindOperator(OperatorDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
            {
                var type = (FunctionType)BindType(syntax.Type, context);
                var kind = syntax.Operator.SyntaxKind;
                if (!structSymbol.Type.AddOperator(kind, type))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, syntax.Operator.Text.ToString());
                return 0;
            }

            static int BindConversion(ConversionDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
            {
                var type = (FunctionType)BindType(syntax.Type, context);
                var kind = syntax.ConversionKeyword.SyntaxKind;
                if (!structSymbol.Type.AddConversion(kind, type))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, syntax.ConversionKeyword.Text.ToString());
                return 0;
            }
        }
    }

    private static FunctionSymbol DeclareFunction(FunctionDeclarationSyntax syntax, BinderContext context)
    {
        var functionName = syntax.IdentifierToken.Text.ToString();
        var functionType = (FunctionType)BindType(syntax.Type, context);
        var functionSymbol = new FunctionSymbol(syntax, functionName, functionType);
        if (!context.BoundScope.Declare(functionSymbol))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, functionName);
        return functionSymbol;
    }

    private static VariableSymbol DeclareVariable(VariableDeclarationSyntax syntax, BinderContext context)
    {
        var variableName = syntax.IdentifierToken.Text.ToString();
        var variableType = BindType(syntax.Type, context);
        var variableSymbol = new VariableSymbol(syntax, variableName, variableType, syntax.IsReadOnly);
        if (!context.BoundScope.Declare(variableSymbol))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, variableName);
        return variableSymbol;
    }
}
