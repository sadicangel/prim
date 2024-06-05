using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static BoundStructDeclaration BindStructDeclaration(StructDeclarationSyntax syntax, BindingContext context)
    {
        var symbolName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not StructSymbol structSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(StructDeclarationSyntax)}'");

        var members = new BoundList<BoundMemberDeclaration>.Builder(structSymbol.Type.Members.Count);
        using (context.PushScope())
        {
            foreach (var member in syntax.Members)
            {
                members.Add(member.SyntaxKind switch
                {
                    SyntaxKind.PropertyDeclaration =>
                        BindPropertyDeclaration(structSymbol.Type, (PropertyDeclarationSyntax)member, context),
                    SyntaxKind.MethodDeclaration =>
                        BindMethodDeclaration(structSymbol.Type, (MethodDeclarationSyntax)member, context),
                    SyntaxKind.OperatorDeclaration =>
                        BindOperatorDeclaration(structSymbol.Type, (OperatorDeclarationSyntax)member, context),
                    _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{member.SyntaxKind}'")
                });
            }
        }
        return new BoundStructDeclaration(syntax, structSymbol, members.ToBoundList());

        static BoundPropertyDeclaration BindPropertyDeclaration(NamedType structType, PropertyDeclarationSyntax syntax, BindingContext context)
        {
            var property = structType.GetProperty(syntax.IdentifierToken.Text)
                ?? throw new UnreachableException($"Unexpected property '{syntax.IdentifierToken.Text}'");
            var propertySymbol = new PropertySymbol(syntax, property);
            if (!context.BoundScope.Declare(propertySymbol))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, propertySymbol.Name);
            // TODO: Allow init expression to be optional.
            var init = BindExpression(syntax.Init, context);

            return new BoundPropertyDeclaration(syntax, propertySymbol, init);
        }

        static BoundMethodDeclaration BindMethodDeclaration(NamedType structType, MethodDeclarationSyntax syntax, BindingContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var method = structType.GetMethod(syntax.IdentifierToken.Text, type)
                ?? throw new UnreachableException($"Unexpected method '{syntax.IdentifierToken.Text}'"); ;
            var methodSymbol = new MethodSymbol(syntax, method);
            if (!context.BoundScope.Declare(methodSymbol))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, methodSymbol.Name);
            var body = BindExpression(syntax.Body, context);
            return new BoundMethodDeclaration(syntax, methodSymbol, body);
        }

        static BoundOperatorDeclaration BindOperatorDeclaration(NamedType structType, OperatorDeclarationSyntax syntax, BindingContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var @operator = structType.GetOperator(syntax.Operator.SyntaxKind, type)
                ?? throw new UnreachableException($"Unexpected operator '{syntax.Operator.Text}'"); ;
            var operatorSymbol = new OperatorSymbol(syntax, @operator);
            if (!context.BoundScope.Declare(operatorSymbol))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, operatorSymbol.Name);
            var body = BindExpression(syntax.Body, context);
            return new BoundOperatorDeclaration(syntax, operatorSymbol, body);
        }
    }
}
