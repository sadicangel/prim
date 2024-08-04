using CodeAnalysis.Syntax;
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
        // error        : '!' type
        // pointer      : '*' type
        // union        : type '|' type
        // array        : '[' type ':' expr ']'
        // lambda       : '(' parameters? ')' '->' type
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
                >= SyntaxKind.AnyKeyword and <= SyntaxKind.F64Keyword =>
                    ParsePredefinedType(syntaxTree, iterator),
                SyntaxKind.IdentifierToken =>
                    ParseNamedType(syntaxTree, iterator),
                SyntaxKind.HookToken =>
                    ParseOptionType(syntaxTree, iterator),
                SyntaxKind.BangToken =>
                    ParseErrorType(syntaxTree, iterator),
                SyntaxKind.StarToken =>
                    ParsePointerType(syntaxTree, iterator),
                SyntaxKind.BracketOpenToken =>
                    ParseArrayType(syntaxTree, iterator),
                SyntaxKind.ParenthesisOpenToken =>
                    ParseLambdaType(syntaxTree, iterator),
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
                var name = ParseSimpleNameExpression(syntaxTree, iterator);
                return new NamedTypeSyntax(syntaxTree, name);
            }

            static (SyntaxToken? ParenthesisOpenToken, TypeSyntax ElementType, SyntaxToken? ParenthesisCloseToken)
                ParseParenthesisedType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var parenthesisOpenToken = default(SyntaxToken);
                var elementType = default(TypeSyntax);
                var parenthesisCloseToken = default(SyntaxToken);
                if (iterator.TryMatch(out parenthesisOpenToken, SyntaxKind.ParenthesisOpenToken))
                {
                    elementType = ParseType(syntaxTree, iterator);
                    parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
                }
                else
                {
                    elementType = ParseTypeSingle(syntaxTree, iterator);
                }

                return (parenthesisOpenToken, elementType, parenthesisCloseToken);
            }

            static OptionTypeSyntax ParseOptionType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var hookToken = iterator.Match(SyntaxKind.HookToken);
                var (parenthesisOpenToken, underlyingType, parenthesisCloseToken) =
                    ParseParenthesisedType(syntaxTree, iterator);

                return new OptionTypeSyntax(syntaxTree, hookToken, parenthesisOpenToken, underlyingType, parenthesisCloseToken);
            }

            static ErrorTypeSyntax ParseErrorType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var hookToken = iterator.Match(SyntaxKind.BangToken);
                var (parenthesisOpenToken, valueType, parenthesisCloseToken) =
                    ParseParenthesisedType(syntaxTree, iterator);

                return new ErrorTypeSyntax(syntaxTree, hookToken, parenthesisOpenToken, valueType, parenthesisCloseToken);
            }

            static PointerTypeSyntax ParsePointerType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var hookToken = iterator.Match(SyntaxKind.StarToken);
                var (parenthesisOpenToken, elementType, parenthesisCloseToken) =
                    ParseParenthesisedType(syntaxTree, iterator);

                return new PointerTypeSyntax(syntaxTree, hookToken, parenthesisOpenToken, elementType, parenthesisCloseToken);
            }

            static ArrayTypeSyntax ParseArrayType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var bracketOpenToken = iterator.Match(SyntaxKind.BracketOpenToken);
                var elementType = ParseType(syntaxTree, iterator);
                var colonToken = iterator.Match(SyntaxKind.ColonToken);
                var length = ParseExpression(syntaxTree, iterator);
                var bracketCloseToken = iterator.Match(SyntaxKind.BracketCloseToken);
                return new ArrayTypeSyntax(syntaxTree, bracketOpenToken, elementType, colonToken, length, bracketCloseToken);
            }

            static LambdaTypeSyntax ParseLambdaType(SyntaxTree syntaxTree, SyntaxIterator iterator)
            {
                var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
                var parameters = ParseSeparatedSyntaxList(
                    syntaxTree,
                    iterator,
                    SyntaxKind.CommaToken,
                    [SyntaxKind.ParenthesisCloseToken, SyntaxKind.EofToken],
                    static (syntaxTree, iterator) =>
                    {
                        var name = ParseSimpleNameExpression(syntaxTree, iterator);
                        var colonToken = iterator.Match(SyntaxKind.ColonToken);
                        var type = ParseType(syntaxTree, iterator);
                        return new ParameterSyntax(syntaxTree, name, colonToken, type);
                    });
                var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
                var arrowToken = iterator.Match(SyntaxKind.MinusGreaterThanToken);
                var returnType = ParseType(syntaxTree, iterator);
                return new LambdaTypeSyntax(syntaxTree, parenthesisOpenToken, parameters, parenthesisCloseToken, arrowToken, returnType);
            }
        }
    }
}
