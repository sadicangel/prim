using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static BoundStructDeclaration BindStructDeclaration(StructDeclarationSyntax syntax, BinderContext context)
    {
        var symbolName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not StructSymbol structSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(StructDeclarationSyntax)}'");

        var members = new BoundList<BoundMemberDeclaration>.Builder(syntax.Members.Count);
        foreach (var member in syntax.Members)
        {
            members.Add(member.SyntaxKind switch
            {
                SyntaxKind.PropertyDeclaration =>
                    BindPropertyDeclaration((PropertyDeclarationSyntax)member, structSymbol, context),
                SyntaxKind.MethodDeclaration =>
                    BindMethodDeclaration((MethodDeclarationSyntax)member, structSymbol, context),
                SyntaxKind.OperatorDeclaration =>
                    BindOperatorDeclaration((OperatorDeclarationSyntax)member, structSymbol, context),
                SyntaxKind.ConversionDeclaration =>
                    BindConversionDeclaration((ConversionDeclarationSyntax)member, structSymbol, context),
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{member.SyntaxKind}'")
            });
        }
        return new BoundStructDeclaration(syntax, structSymbol, members.ToBoundList());

        static BoundPropertyDeclaration BindPropertyDeclaration(PropertyDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
        {
            var property = structSymbol.Type.GetProperty(syntax.IdentifierToken.Text)
                ?? throw new UnreachableException($"Unexpected property '{syntax.IdentifierToken.Text}'");

            // TODO: Allow init expression to be optional, if property is optional.
            var init = Coerce(BindExpression(syntax.Init, context), property.Type, context);

            var propertySymbol = new PropertySymbol(syntax, property, syntax.IsReadOnly);

            return new BoundPropertyDeclaration(syntax, propertySymbol, init);
        }

        static BoundMethodDeclaration BindMethodDeclaration(MethodDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var method = structSymbol.Type.GetMethod(syntax.IdentifierToken.Text, type)
                ?? throw new UnreachableException($"Unexpected method '{syntax.IdentifierToken.Text}'"); ;

            var body = BindExpression(syntax.Body, context);

            var methodSymbol = new MethodSymbol(syntax, method);

            return new BoundMethodDeclaration(syntax, methodSymbol, body);
        }

        static BoundOperatorDeclaration BindOperatorDeclaration(OperatorDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var @operator = structSymbol.Type.GetOperator(syntax.Operator.SyntaxKind, type)
                ?? throw new UnreachableException($"Unexpected operator '{syntax.Operator.Text}'"); ;

            var body = BindExpression(syntax.Body, context);

            var operatorSymbol = new OperatorSymbol(syntax, @operator);

            return new BoundOperatorDeclaration(syntax, operatorSymbol, body);
        }

        static BoundConversionDeclaration BindConversionDeclaration(ConversionDeclarationSyntax syntax, StructSymbol structSymbol, BinderContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var conversion = structSymbol.Type.GetConversion(type)
                ?? throw new UnreachableException($"Unexpected conversion '{type}'"); ;

            var body = BindExpression(syntax.Body, context);

            var conversionSymbol = new ConversionSymbol(syntax, conversion);

            return new BoundConversionDeclaration(syntax, conversionSymbol, body);
        }
    }
}
