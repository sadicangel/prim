using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static IEnumerable<(DeclarationSyntax, SyntaxKind)> GetDeclarations(SyntaxNode syntax) =>
        syntax.Children().OfType<DeclarationSyntax>().Select(s => (s, s.SyntaxKind));

    public static void Declare_StepOne(CompilationUnitSyntax syntax, BinderContext context)
    {
        var queue = new PriorityQueue<DeclarationSyntax, SyntaxKind>(GetDeclarations(syntax));
        while (queue.Count > 0)
        {
            var declaration = queue.Dequeue();
            Declare_StepOne(declaration, context);
        }
    }

    public static void Declare_StepTwo(CompilationUnitSyntax syntax, BinderContext context)
    {
        var queue = new PriorityQueue<DeclarationSyntax, SyntaxKind>(GetDeclarations(syntax));
        while (queue.Count > 0)
        {
            var declaration = queue.Dequeue();
            Declare_StepTwo(declaration, context);
        }
    }

    private static void Declare_StepOne(DeclarationSyntax declaration, BinderContext context)
    {
        switch (declaration.SyntaxKind)
        {
            case SyntaxKind.StructDeclaration:
                {
                    var structDeclaration = (StructDeclarationSyntax)declaration;
                    var structName = structDeclaration.Name.Text.ToString();
                    var typeSymbol = new StructTypeSymbol(structDeclaration, structName);
                    if (!context.BoundScope.Declare(typeSymbol))
                        context.Diagnostics.ReportSymbolRedeclaration(structDeclaration.Location, structName);
                }
                break;
            case SyntaxKind.VariableDeclaration:
                {
                    var variableDeclaration = (VariableDeclarationSyntax)declaration;
                    var variableName = variableDeclaration.Name.Text.ToString();
                    var variableType = variableDeclaration.Type is null
                        ? PredefinedSymbols.Unknown
                        : BindType(variableDeclaration.Type, context);
                    var variableSymbol = new VariableSymbol(
                        variableDeclaration,
                        variableName,
                        variableType,
                        IsStatic: true,
                        variableDeclaration.IsReadOnly);

                    if (!context.BoundScope.Declare(variableSymbol))
                        context.Diagnostics.ReportSymbolRedeclaration(variableDeclaration.Location, variableName);
                }
                break;
            default:
                throw new UnreachableException($"Unexpected declaration '{declaration.SyntaxKind}'");
        }
    }

    private static void Declare_StepTwo(DeclarationSyntax declaration, BinderContext context)
    {
        switch (declaration.SyntaxKind)
        {
            case SyntaxKind.StructDeclaration:
                {
                    var structDeclaration = (StructDeclarationSyntax)declaration;
                    var structName = structDeclaration.Name.Text.ToString();
                    if (context.BoundScope.Lookup(structName) is not StructTypeSymbol structType)
                        throw new UnreachableException($"Unexpected declaration '{declaration}'");
                    foreach (var member in structDeclaration.Members)
                    {
                        switch (member.SyntaxKind)
                        {
                            case SyntaxKind.PropertyDeclaration:
                                {
                                    var propertyDeclaration = (PropertyDeclarationSyntax)member;
                                    var name = propertyDeclaration.Name.Text.ToString();
                                    var type = BindType(propertyDeclaration.Type, context);
                                    if (!structType.AddProperty(name, type, propertyDeclaration))
                                        context.Diagnostics.ReportSymbolRedeclaration(propertyDeclaration.Location, name);
                                }
                                break;
                            case SyntaxKind.MethodDeclaration:
                                {
                                    var methodDeclaration = (MethodDeclarationSyntax)member;
                                    var name = methodDeclaration.Name.Text.ToString();
                                    var type = BindLambdaType(methodDeclaration.Type, context);
                                    if (!structType.AddMethod(name, type, methodDeclaration))
                                        context.Diagnostics.ReportSymbolRedeclaration(methodDeclaration.Location, name);
                                }
                                break;
                            case SyntaxKind.OperatorDeclaration:
                                {
                                    var operatorDeclaration = (OperatorDeclarationSyntax)member;
                                    var name = SyntaxFacts.GetText(operatorDeclaration.OperatorToken.SyntaxKind)
                                        ?? throw new UnreachableException($"Unexpected operator '{operatorDeclaration.OperatorToken}'");
                                    var type = BindLambdaType(operatorDeclaration.Type, context);
                                    if (!structType.AddOperator(type, operatorDeclaration))
                                        context.Diagnostics.ReportSymbolRedeclaration(operatorDeclaration.Location, name);
                                }
                                break;
                            case SyntaxKind.ConversionDeclaration:
                                {
                                    var conversionDeclaration = (ConversionDeclarationSyntax)member;
                                    var name = SyntaxFacts.GetText(conversionDeclaration.ConversionKeyword.SyntaxKind)
                                        ?? throw new UnreachableException($"Unexpected operator '{conversionDeclaration.ConversionKeyword}'");
                                    var type = BindLambdaType(conversionDeclaration.Type, context);
                                    if (!structType.AddConversion(type, conversionDeclaration))
                                        context.Diagnostics.ReportSymbolRedeclaration(conversionDeclaration.Location, name);
                                }
                                break;
                        }
                    }
                }
                break;
            case SyntaxKind.VariableDeclaration:
                break;
            default:
                throw new UnreachableException($"Unexpected declaration '{declaration.SyntaxKind}'");
        }
    }
}
