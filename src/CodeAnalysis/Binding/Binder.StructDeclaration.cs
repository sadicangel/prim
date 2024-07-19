﻿using System.Diagnostics;
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
        if (context.BoundScope.Lookup(symbolName) is not TypeSymbol typeSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(StructDeclarationSyntax)}'");

        var members = new BoundList<BoundMemberDeclaration>.Builder(syntax.Members.Count);
        foreach (var member in syntax.Members)
        {
            members.Add(member.SyntaxKind switch
            {
                SyntaxKind.PropertyDeclaration =>
                    BindPropertyDeclaration((PropertyDeclarationSyntax)member, typeSymbol, context),
                SyntaxKind.MethodDeclaration =>
                    BindMethodDeclaration((MethodDeclarationSyntax)member, typeSymbol, context),
                SyntaxKind.OperatorDeclaration =>
                    BindOperatorDeclaration((OperatorDeclarationSyntax)member, typeSymbol, context),
                SyntaxKind.ConversionDeclaration =>
                    BindConversionDeclaration((ConversionDeclarationSyntax)member, typeSymbol, context),
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{member.SyntaxKind}'")
            });
        }
        return new BoundStructDeclaration(syntax, typeSymbol, members.ToBoundList());

        static BoundPropertyDeclaration BindPropertyDeclaration(PropertyDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
        {
            var property = typeSymbol.Type.GetProperty(syntax.IdentifierToken.Text)
                ?? throw new UnreachableException($"Unexpected property '{syntax.IdentifierToken.Text}'");

            // TODO: Allow init expression to be optional, if property is optional.
            var init = Coerce(BindExpression(syntax.Init, context), property.Type, context);

            var propertySymbol = PropertySymbol.FromProperty(property, syntax);

            return new BoundPropertyDeclaration(syntax, propertySymbol, init);
        }

        static BoundMethodDeclaration BindMethodDeclaration(MethodDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var method = typeSymbol.Type.GetMethod(syntax.IdentifierToken.Text, type)
                ?? throw new UnreachableException($"Unexpected method '{syntax.IdentifierToken.Text}'");

            var methodSymbol = MethodSymbol.FromMethod(method, typeSymbol.Type, syntax);

            var body = BindMethodBody(syntax.Body, methodSymbol, context);

            return new BoundMethodDeclaration(syntax, methodSymbol, body);
        }

        static BoundOperatorDeclaration BindOperatorDeclaration(OperatorDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var @operator = typeSymbol.Type.GetOperator(syntax.OperatorToken.SyntaxKind, type)
                ?? throw new UnreachableException($"Unexpected operator '{syntax.OperatorToken.Text}'");

            var methodSymbol = MethodSymbol.FromOperator(@operator, syntax);

            var body = BindMethodBody(syntax.Body, methodSymbol, context);

            return new BoundOperatorDeclaration(syntax, methodSymbol, body);
        }

        static BoundConversionDeclaration BindConversionDeclaration(ConversionDeclarationSyntax syntax, TypeSymbol typeSymbol, BinderContext context)
        {
            var type = (FunctionType)BindType(syntax.Type, context);
            var conversion = typeSymbol.Type.GetConversion(type)
                ?? throw new UnreachableException($"Unexpected conversion '{type}'");

            // TODO: Either here or when declaring, must ensure only 1 parameter.
            var methodSymbol = MethodSymbol.FromConversion(conversion, syntax);

            var body = BindMethodBody(syntax.Body, methodSymbol, context);

            return new BoundConversionDeclaration(syntax, methodSymbol, body);
        }
    }
}
