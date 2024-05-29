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

    //private static Symbol? DeclareType(TypeSyntax type, BindingContext context)
    //{
    //    switch (type.SyntaxKind)
    //    {
    //        case SyntaxKind.PredefinedType:
    //            return null;

    //        case SyntaxKind.NamedType:
    //            var namedTypeName = ((NamedTypeSyntax)type).IdentifierToken.Text.ToString();
    //            return context.BoundScope.Lookup(namedTypeName)
    //                ?? throw new InvalidOperationException($"Missing compiler defined type '{namedTypeName}'");

    //        case SyntaxKind.OptionType:
    //            return null;

    //        case SyntaxKind.ArrayType:
    //            return null;

    //        case SyntaxKind.FunctionType:


    //        case SyntaxKind.UnionType:
    //            break;
    //        case SyntaxKind.Parameter:
    //            break;
    //        case SyntaxKind.ParameterList:
    //            break;
    //        case SyntaxKind.TypeList:
    //            break;
    //        default:
    //            throw new UnreachableException($"Unexpected {nameof(TypeSyntax)} '{type.GetType()}'");
    //    }

    //    static Prede BindPredefinedTypeSymbol(PredefinedTypeSyntax type, BindingContext context)
    //    {
    //        return type.PredefinedTypeToken.SyntaxKind switch
    //        {
    //            SyntaxKind.AnyKeyword => PredefinedTypes.Any,
    //            SyntaxKind.UnknownKeyword => PredefinedTypes.Unknown,
    //            SyntaxKind.NeverKeyword => PredefinedTypes.Never,
    //            SyntaxKind.UnitKeyword => PredefinedTypes.Unit,
    //            SyntaxKind.TypeKeyword => PredefinedTypes.Type,
    //            SyntaxKind.StrKeyword => PredefinedTypes.Str,
    //            SyntaxKind.BoolKeyword => PredefinedTypes.Bool,
    //            SyntaxKind.I8Keyword => PredefinedTypes.I8,
    //            SyntaxKind.I16Keyword => PredefinedTypes.I16,
    //            SyntaxKind.I32Keyword => PredefinedTypes.I32,
    //            SyntaxKind.I64Keyword => PredefinedTypes.I64,
    //            SyntaxKind.I128Keyword => PredefinedTypes.I128,
    //            SyntaxKind.ISizeKeyword => PredefinedTypes.ISize,
    //            SyntaxKind.U8Keyword => PredefinedTypes.U8,
    //            SyntaxKind.U16Keyword => PredefinedTypes.U16,
    //            SyntaxKind.U32Keyword => PredefinedTypes.U32,
    //            SyntaxKind.U64Keyword => PredefinedTypes.U64,
    //            SyntaxKind.U128Keyword => PredefinedTypes.U128,
    //            SyntaxKind.USizeKeyword => PredefinedTypes.USize,
    //            SyntaxKind.F16Keyword => PredefinedTypes.F16,
    //            SyntaxKind.F32Keyword => PredefinedTypes.F32,
    //            SyntaxKind.F64Keyword => PredefinedTypes.F64,
    //            SyntaxKind.F80Keyword => PredefinedTypes.F80,
    //            SyntaxKind.F128Keyword => PredefinedTypes.F128,
    //            _ => throw new UnreachableException($"Unexpected predefined type '{type.PredefinedTypeToken.SyntaxKind}'")
    //        };
    //    }

    //    static NamedType BindNamedType(NamedTypeSyntax type, BindingContext context)
    //    {
    //        context.BoundScope.Lookup(type.Text)
    //            }

    //    switch (type)
    //    {
    //        case ArrayTypeSyntax arrayType:
    //            return new ArrayType()



    //        case FunctionTypeSyntax functionType:
    //            return new FunctionType(
    //                [.. functionType.Parameters.Select(p => new Parameter(p.Text.ToString(), BindType(p.Type)))],
    //                BindType(functionType.ReturnType));
    //        case NamedTypeSyntax namedType:
    //            break;
    //        case OptionTypeSyntax optionType:
    //            break;
    //        case PredefinedTypeSyntax predefinedType:
    //            return predefinedType.PredefinedTypeToken.SyntaxKind switch
    //            {
    //                SyntaxKind.AnyKeyword => PredefinedTypes.Any,
    //                SyntaxKind.UnknownKeyword => PredefinedTypes.Unknown,
    //                SyntaxKind.NeverKeyword => PredefinedTypes.Never,
    //                SyntaxKind.UnitKeyword => PredefinedTypes.Unit,
    //                SyntaxKind.TypeKeyword => PredefinedTypes.Type,
    //                SyntaxKind.StrKeyword => PredefinedTypes.Str,
    //                SyntaxKind.BoolKeyword => PredefinedTypes.Bool,
    //                SyntaxKind.I8Keyword => PredefinedTypes.I8,
    //                SyntaxKind.I16Keyword => PredefinedTypes.I16,
    //                SyntaxKind.I32Keyword => PredefinedTypes.I32,
    //                SyntaxKind.I64Keyword => PredefinedTypes.I64,
    //                SyntaxKind.I128Keyword => PredefinedTypes.I128,
    //                SyntaxKind.ISizeKeyword => PredefinedTypes.ISize,
    //                SyntaxKind.U8Keyword => PredefinedTypes.U8,
    //                SyntaxKind.U16Keyword => PredefinedTypes.U16,
    //                SyntaxKind.U32Keyword => PredefinedTypes.U32,
    //                SyntaxKind.U64Keyword => PredefinedTypes.U64,
    //                SyntaxKind.U128Keyword => PredefinedTypes.U128,
    //                SyntaxKind.USizeKeyword => PredefinedTypes.USize,
    //                SyntaxKind.F16Keyword => PredefinedTypes.F16,
    //                SyntaxKind.F32Keyword => PredefinedTypes.F32,
    //                SyntaxKind.F64Keyword => PredefinedTypes.F64,
    //                SyntaxKind.F80Keyword => PredefinedTypes.F80,
    //                SyntaxKind.F128Keyword => PredefinedTypes.F128,
    //                _ => throw new UnreachableException($"Unexpected predefined type '{predefinedType.PredefinedTypeToken.SyntaxKind}'")
    //            };
    //        case UnionTypeSyntax unionType:
    //            break;
    //        default:
    //            throw new NotImplementedException(type.GetType().Name);
    //    }
    //}
}
