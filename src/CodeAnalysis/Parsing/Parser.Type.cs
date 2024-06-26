﻿using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static TypeSyntax ParseType(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        // type         : predefined | named | option | union | array | function
        // predefined   : identifier (predefined names)
        // named        : identifier
        // option       : '?' type 
        // union        : type '|' type
        // array        : '[' type ':' expr ']'
        // function     : '(' parameters? ')' '->' type
        // parameters   : parameter [',' parameter]
        // parameter    : identifier ':' type

        // TODO: Avoid creating a list every time.
        var types = ParseSeparatedSyntaxList(
            syntaxTree,
            iterator,
            SyntaxKind.PipeToken,
            [SyntaxKind.ColonToken, SyntaxKind.EqualsToken, SyntaxKind.EofToken],
            ParseTypeSingle);

        return types switch
        {
            { Count: > 1 } => new UnionTypeSyntax(syntaxTree, types),
            [TypeSyntax type] => type,
            _ => ReportInvalidType(syntaxTree, iterator)
        };

        static TypeSyntax ReportInvalidType(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            syntaxTree.Diagnostics.ReportExpectedTypeDefinition(iterator.Current.Location);
            return new PredefinedTypeSyntax(syntaxTree, iterator.Current);
        }

        static TypeSyntax ParseTypeSingle(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            return iterator.Current.SyntaxKind switch
            {
                >= SyntaxKind.AnyKeyword and <= SyntaxKind.F128Keyword =>
                    ParsePredefinedType(syntaxTree, iterator),
                SyntaxKind.IdentifierToken =>
                    ParseNamedType(syntaxTree, iterator),
                SyntaxKind.HookToken =>
                    ParseOptionType(syntaxTree, iterator),
                SyntaxKind.BracketOpenToken =>
                    ParseArrayType(syntaxTree, iterator),
                SyntaxKind.ParenthesisOpenToken =>
                    ParseFunctionType(syntaxTree, iterator),
                _ =>
                    ReportInvalidType(syntaxTree, iterator),
            };

            static PredefinedTypeSyntax ParsePredefinedType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var predefinedTypeToken = iterator.Match();
                return new PredefinedTypeSyntax(syntaxTree, predefinedTypeToken);
            }

            static NamedTypeSyntax ParseNamedType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
                return new NamedTypeSyntax(syntaxTree, identifierToken);
            }

            static OptionTypeSyntax ParseOptionType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var hookToken = iterator.Match(SyntaxKind.HookToken);
                var underlyingType = ParseTypeSingle(syntaxTree, iterator);
                return new OptionTypeSyntax(syntaxTree, hookToken, underlyingType);
            }

            static ArrayTypeSyntax ParseArrayType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var bracketOpenToken = iterator.Match(SyntaxKind.BracketOpenToken);
                var elementType = ParseTypeSingle(syntaxTree, iterator);
                var colonToken = iterator.Match(SyntaxKind.ColonToken);
                var length = ParseExpression(syntaxTree, iterator);
                var bracketCloseToken = iterator.Match(SyntaxKind.BracketCloseToken);
                return new ArrayTypeSyntax(syntaxTree, bracketOpenToken, elementType, colonToken, length, bracketCloseToken);
            }

            static FunctionTypeSyntax ParseFunctionType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
                var parameters = ParseSeparatedSyntaxList(
                    syntaxTree,
                    iterator,
                    SyntaxKind.CommaToken,
                    [SyntaxKind.ParenthesisCloseToken, SyntaxKind.EofToken],
                    static (syntaxTree, iterator) =>
                    {
                        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
                        var colonToken = iterator.Match(SyntaxKind.ColonToken);
                        var type = ParseTypeSingle(syntaxTree, iterator);
                        return new ParameterSyntax(syntaxTree, identifierToken, colonToken, type);
                    });
                var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
                var arrowToken = iterator.Match(SyntaxKind.MinusGreaterThanToken);
                var returnType = ParseTypeSingle(syntaxTree, iterator);
                return new FunctionTypeSyntax(syntaxTree, parenthesisOpenToken, parameters, parenthesisCloseToken, arrowToken, returnType);
            }
        }
    }
}
