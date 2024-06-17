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
        var context = new BinderContext(boundTree, boundScope);

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

    private static Symbol Declare(DeclarationSyntax syntax, BinderContext context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.VariableDeclaration => DeclareVariable((VariableDeclarationSyntax)syntax, context),
            SyntaxKind.FunctionDeclaration => DeclareFunction((FunctionDeclarationSyntax)syntax, context),
            SyntaxKind.StructDeclaration => DeclareStruct((StructDeclarationSyntax)syntax, context),
            _ => throw new UnreachableException($"Unexpected {nameof(DeclarationSyntax)} '{syntax.GetType().Name}'")
        };
    }

    private static StructSymbol DeclareStruct(StructDeclarationSyntax syntax, BinderContext context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        var structSymbol = new StructSymbol(syntax, new StructType(structName));
        if (!context.BoundScope.Declare(structSymbol))
            context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, structName);
        return structSymbol;
    }

    private static void DeclareStructMembers(StructDeclarationSyntax syntax, BinderContext context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(structName) is not StructSymbol structSymbol)
            throw new UnreachableException($"Type '{structName}' was not declared");

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

        static int BindProperty(PropertyDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
        {
            var name = syntax.IdentifierToken.Text.ToString();
            var type = BindType(syntax.Type, context);
            if (!structSymbol.Type.AddProperty(new Property(name, type, syntax.IsReadOnly)))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, name);

            return 0;
        }

        static int BindMethod(MethodDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
        {
            var name = syntax.IdentifierToken.Text.ToString();
            var type = (FunctionType)BindType(syntax.Type, context);
            if (!structSymbol.Type.AddMethod(new Method(name, type)))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, name);
            return 0;
        }

        static int BindOperator(OperatorDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var kind = syntax.Operator.SyntaxKind;
            if (!structSymbol.Type.AddOperator(new Operator(kind, type)))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, syntax.Operator.Text.ToString());
            return 0;
        }

        static int BindConversion(ConversionDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var kind = syntax.ConversionKeyword.SyntaxKind;
            if (!structSymbol.Type.AddConversion(new Conversion(kind, type)))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, syntax.ConversionKeyword.Text.ToString());
            return 0;
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
