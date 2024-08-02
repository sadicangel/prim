using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    public static void Declare_StepOne(CompilationUnitSyntax syntax, Context context)
    {
        foreach (var declaration in syntax.Children().OfType<DeclarationSyntax>())
            Declare_StepOne(declaration, context);
    }

    public static void Declare_StepTwo(CompilationUnitSyntax syntax, Context context)
    {
        foreach (var declaration in syntax.Children().OfType<DeclarationSyntax>())
            Declare_StepTwo(declaration, context);
    }

    private static void Declare_StepOne(DeclarationSyntax syntax, Context context)
    {
        switch (syntax.SyntaxKind)
        {
            case SyntaxKind.ModuleDeclaration:
                {
                    var moduleDeclaration = (ModuleDeclarationSyntax)syntax;
                    var moduleName = moduleDeclaration.Name.Text.ToString();
                    var moduleSymbol = new ModuleSymbol(moduleDeclaration, moduleName, context.BoundScope.Never, context.Module);
                    if (!context.Module.Declare(moduleSymbol))
                        context.Diagnostics.ReportSymbolRedeclaration(moduleDeclaration.Location, moduleName);
                    using (context.PushBoundScope(moduleSymbol))
                    {
                        foreach (var declaration in syntax.Children().OfType<DeclarationSyntax>())
                            Declare_StepOne(declaration, context);
                    }
                }
                break;
            case SyntaxKind.StructDeclaration:
                {
                    var structDeclaration = (StructDeclarationSyntax)syntax;
                    var structName = structDeclaration.Name.Text.ToString();
                    var typeSymbol = new StructTypeSymbol(structDeclaration, structName, context.BoundScope.RuntimeType, context.Module);
                    if (!context.Module.Declare(typeSymbol))
                        context.Diagnostics.ReportSymbolRedeclaration(structDeclaration.Location, structName);
                }
                break;
            case SyntaxKind.VariableDeclaration:
                break;
            default:
                throw new UnreachableException($"Unexpected declaration '{syntax.SyntaxKind}'");
        }
    }

    private static void Declare_StepTwo(DeclarationSyntax syntax, Context context)
    {
        switch (syntax.SyntaxKind)
        {
            case SyntaxKind.ModuleDeclaration:
                var moduleDeclaration = (ModuleDeclarationSyntax)syntax;
                var moduleName = moduleDeclaration.Name.Text.ToString();
                if (context.BoundScope.Lookup(moduleName) is not ModuleSymbol moduleSymbol)
                    throw new UnreachableException($"Unexpected declaration '{syntax}'");
                using (context.PushBoundScope(moduleSymbol))
                {
                    foreach (var declaration in syntax.Children().OfType<DeclarationSyntax>())
                        Declare_StepTwo(declaration, context);
                }
                break;
            case SyntaxKind.StructDeclaration:
                {
                    var structDeclaration = (StructDeclarationSyntax)syntax;
                    var structName = structDeclaration.Name.Text.ToString();
                    if (context.BoundScope.Lookup(structName) is not StructTypeSymbol structType)
                        throw new UnreachableException($"Unexpected declaration '{syntax}'");
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
                                    switch (type.Parameters.Count)
                                    {
                                        case 1 when !SyntaxFacts.IsUnaryOperator(operatorDeclaration.OperatorToken.SyntaxKind):
                                            context.Diagnostics.ReportInvalidOperatorDeclaration(operatorDeclaration.Location, "unary", "1");
                                            break;
                                        case 2 when !SyntaxFacts.IsBinaryOperator(operatorDeclaration.OperatorToken.SyntaxKind):
                                            context.Diagnostics.ReportInvalidOperatorDeclaration(operatorDeclaration.Location, "binary", "2");
                                            break;
                                        default:
                                            context.Diagnostics.ReportInvalidOperatorDeclaration(operatorDeclaration.Location, "unary or binary", "1 or 2");
                                            break;
                                    }
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
                                    if (type.Parameters.Count is not 1 || (type.Parameters[0].Type != structType && type.ReturnType != structType))
                                    {
                                        context.Diagnostics.ReportInvalidConversionDeclaration(conversionDeclaration.Location);
                                    }
                                    if (!structType.AddConversion(type, conversionDeclaration))
                                        context.Diagnostics.ReportSymbolRedeclaration(conversionDeclaration.Location, name);
                                }
                                break;
                        }
                    }
                }
                break;
            case SyntaxKind.VariableDeclaration:
                {
                    var variableDeclaration = (VariableDeclarationSyntax)syntax;
                    var variableName = variableDeclaration.Name.Text.ToString();
                    var variableType = variableDeclaration.Type is null
                        ? context.BoundScope.Unknown
                        : BindType(variableDeclaration.Type, context);
                    var variableSymbol = new VariableSymbol(
                        variableDeclaration,
                        variableName,
                        variableType,
                        context.Module,
                        IsStatic: true,
                        variableDeclaration.IsReadOnly);

                    if (!context.BoundScope.Declare(variableSymbol))
                        context.Diagnostics.ReportSymbolRedeclaration(variableDeclaration.Location, variableName);
                }
                break;
            default:
                throw new UnreachableException($"Unexpected declaration '{syntax.SyntaxKind}'");
        }
    }
}
