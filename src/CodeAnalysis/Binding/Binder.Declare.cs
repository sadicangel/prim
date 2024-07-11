using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static Symbol Declare(DeclarationSyntax syntax, BinderContext context, bool isTopLevel)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.VariableDeclaration => DeclareVariable((VariableDeclarationSyntax)syntax, context),
            SyntaxKind.FunctionDeclaration => DeclareFunction((FunctionDeclarationSyntax)syntax, context, isTopLevel),
            SyntaxKind.StructDeclaration => DeclareStruct((StructDeclarationSyntax)syntax, context, isTopLevel),
            _ => throw new UnreachableException($"Unexpected {nameof(DeclarationSyntax)} '{syntax.GetType().Name}'")
        };
    }

    private static TypeSymbol DeclareStruct(StructDeclarationSyntax syntax, BinderContext context, bool isTopLevel)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        if (isTopLevel)
        {
            var typeSymbol = new TypeSymbol(
                syntax,
                new StructType(structName),
                NamespaceSymbol.Global,
                NamespaceSymbol.Global);
            if (!context.BoundScope.Declare(typeSymbol))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, structName);
            return typeSymbol;
        }
        else
        {
            if (context.BoundScope.Lookup(structName) is not TypeSymbol typeSymbol)
            {
                typeSymbol = new TypeSymbol(
                    syntax,
                    new StructType(structName),
                    NamespaceSymbol.Global,
                    NamespaceSymbol.Global);
                if (!context.BoundScope.Declare(typeSymbol))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, structName);
            }

            foreach (var memberSyntax in syntax.Members)
            {
                _ = memberSyntax.SyntaxKind switch
                {
                    SyntaxKind.PropertyDeclaration => BindProperty((PropertyDeclarationSyntax)memberSyntax, typeSymbol, context),
                    SyntaxKind.MethodDeclaration => BindMethod((MethodDeclarationSyntax)memberSyntax, typeSymbol, context),
                    SyntaxKind.OperatorDeclaration => BindOperator((OperatorDeclarationSyntax)memberSyntax, typeSymbol, context),
                    SyntaxKind.ConversionDeclaration => BindConversion((ConversionDeclarationSyntax)memberSyntax, typeSymbol, context),
                    _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{memberSyntax.SyntaxKind}'")
                };
            }

            return typeSymbol;


            static int BindProperty(PropertyDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
            {
                var name = syntax.IdentifierToken.Text.ToString();
                var type = BindType(syntax.Type, context);
                if (!typeSymbol.Type.AddProperty(name, type, syntax.IsReadOnly))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, name);

                return 0;
            }

            static int BindMethod(MethodDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
            {
                var name = syntax.IdentifierToken.Text.ToString();
                var type = (FunctionType)BindType(syntax.Type, context);
                if (!typeSymbol.Type.AddMethod(name, type))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, name);
                return 0;
            }

            static int BindOperator(OperatorDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
            {
                var type = (FunctionType)BindType(syntax.Type, context);
                var kind = syntax.OperatorToken.SyntaxKind;
                if (!typeSymbol.Type.AddOperator(kind, type))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, syntax.OperatorToken.Text.ToString());
                return 0;
            }

            static int BindConversion(ConversionDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
            {
                var type = (FunctionType)BindType(syntax.Type, context);
                var kind = syntax.ConversionKeyword.SyntaxKind;
                if (!typeSymbol.Type.AddConversion(kind, type))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, syntax.ConversionKeyword.Text.ToString());
                return 0;
            }
        }
    }

    private static FunctionSymbol DeclareFunction(FunctionDeclarationSyntax syntax, BinderContext context, bool isTopLevel)
    {
        if (isTopLevel && !syntax.IsReadOnly)
            context.Diagnostics.ReportMutableGlobalDeclaration(syntax.Location, "function");

        var parameterSymbols = new BoundList<VariableSymbol>.Builder();
        var seenParameterNames = new HashSet<string>();
        foreach (var parameterSyntax in syntax.Type.Parameters)
        {
            var parameterName = parameterSyntax.IdentifierToken.Text.ToString();
            if (!seenParameterNames.Add(parameterName))
                context.Diagnostics.ReportSymbolRedeclaration(parameterSyntax.Location, parameterName);
            var parameterType = BindType(parameterSyntax.Type, context);
            var parameterSymbol = new VariableSymbol(
                parameterSyntax,
                parameterName,
                parameterType,
                NamespaceSymbol.Global,
                NamespaceSymbol.Global,
                IsReadOnly: false);
            parameterSymbols.Add(parameterSymbol);
        }
        var functionName = syntax.IdentifierToken.Text.ToString();
        var functionReturnType = BindType(syntax.Type.ReturnType, context);
        var functionType = new FunctionType([.. parameterSymbols.Select(p => new Parameter(p.Name, p.Type))], functionReturnType);
        var functionSymbol = new FunctionSymbol(
            syntax,
            functionName,
            functionType,
            NamespaceSymbol.Global,
            NamespaceSymbol.Global,
            IsReadOnly: isTopLevel || syntax.IsReadOnly,
            IsStatic: true,
            []);

        if (!context.BoundScope.Declare(functionSymbol))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, functionName);

        return functionSymbol;
    }

    private static VariableSymbol DeclareVariable(VariableDeclarationSyntax syntax, BinderContext context)
    {
        var variableName = syntax.IdentifierToken.Text.ToString();
        var variableType = syntax.Type is null ? PredefinedTypes.Unknown : BindType(syntax.Type, context);
        var variableSymbol = new VariableSymbol(
            syntax,
            variableName,
            variableType,
            NamespaceSymbol.Global,
            NamespaceSymbol.Global,
            syntax.IsReadOnly);
        if (!context.BoundScope.Declare(variableSymbol))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, variableName);
        return variableSymbol;
    }
}
