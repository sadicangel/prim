using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static TypeSyntax ParseType(SyntaxIterator iterator)
    {
        // type         : predefined | named | maybe | array | pointer | lambda | union 
        // predefined   : identifier (predefined names)
        // named        : identifier
        // maybe        : type '?'
        // array        : type '[' expr ']'
        // pointer      : type '*'
        // lambda       : '(' parameters? ')' '->' type
        // parameters   : parameter [',' parameters]*
        // parameter    : identifier ':' type
        // union        : type '|' type ['|' union]*

        var types = ParseSyntaxList(
            iterator,
            SyntaxKind.PipeToken,
            [SyntaxKind.ColonToken, SyntaxKind.EqualsToken],
            ParseSingleType);

        return types is [var type] ? type : new UnionTypeSyntax(types);

        static TypeSyntax ParseSingleType(SyntaxIterator iterator)
        {
            TypeSyntax type = iterator.Current.SyntaxKind switch
            {
                SyntaxKind.AnyKeyword or
                    SyntaxKind.UnknownKeyword or
                    SyntaxKind.NeverKeyword or
                    SyntaxKind.UnitKeyword or
                    SyntaxKind.TypeKeyword or
                    SyntaxKind.StrKeyword or
                    SyntaxKind.BoolKeyword or
                    SyntaxKind.I8Keyword or
                    SyntaxKind.I16Keyword or
                    SyntaxKind.I32Keyword or
                    SyntaxKind.I64Keyword or
                    SyntaxKind.IszKeyword or
                    SyntaxKind.U8Keyword or
                    SyntaxKind.U16Keyword or
                    SyntaxKind.U32Keyword or
                    SyntaxKind.U64Keyword or
                    SyntaxKind.UszKeyword or
                    SyntaxKind.F16Keyword or
                    SyntaxKind.F32Keyword or
                    SyntaxKind.F64Keyword => ParsePredefinedType(iterator),

                SyntaxKind.ParenthesisOpenToken => ParseLambdaType(iterator),

                _ => ParseNamedType(iterator)
            };

            return iterator.Current.SyntaxKind switch
            {
                SyntaxKind.BracketOpenToken => ParseArrayType(iterator, type),
                SyntaxKind.StarToken => ParsePointerType(iterator, type),
                SyntaxKind.HookToken => ParseMaybeType(iterator, type),
                _ => type,
            };
        }

        static PredefinedTypeSyntax ParsePredefinedType(SyntaxIterator iterator)
        {
            var predefinedTypeToken = iterator.Match(
            [
                SyntaxKind.AnyKeyword,
                SyntaxKind.UnknownKeyword,
                SyntaxKind.NeverKeyword,
                SyntaxKind.UnitKeyword,
                SyntaxKind.TypeKeyword,
                SyntaxKind.StrKeyword,
                SyntaxKind.BoolKeyword,
                SyntaxKind.I8Keyword,
                SyntaxKind.I16Keyword,
                SyntaxKind.I32Keyword,
                SyntaxKind.I64Keyword,
                SyntaxKind.IszKeyword,
                SyntaxKind.U8Keyword,
                SyntaxKind.U16Keyword,
                SyntaxKind.U32Keyword,
                SyntaxKind.U64Keyword,
                SyntaxKind.UszKeyword,
                SyntaxKind.F16Keyword,
                SyntaxKind.F32Keyword,
                SyntaxKind.F64Keyword
            ]);

            return new PredefinedTypeSyntax(predefinedTypeToken);
        }

        static LambdaTypeSyntax ParseLambdaType(SyntaxIterator iterator)
        {
            var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
            var parameters = ParseSyntaxList(
                iterator,
                SyntaxKind.CommaToken,
                [SyntaxKind.ParenthesisCloseToken, SyntaxKind.EofToken],
                ParseType);
            var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
            var arrowToken = iterator.Match(SyntaxKind.MinusGreaterThanToken);
            var returnType = ParseType(iterator);
            return new LambdaTypeSyntax(parenthesisOpenToken, parameters, parenthesisCloseToken, arrowToken, returnType);
        }

        static NamedTypeSyntax ParseNamedType(SyntaxIterator iterator)
        {
            var name = ParseName(iterator);
            return new NamedTypeSyntax(name);
        }

        static ArrayTypeSyntax ParseArrayType(SyntaxIterator iterator, TypeSyntax elementType)
        {
            var bracketOpenToken = iterator.Match(SyntaxKind.BracketOpenToken);
            if (iterator.TryMatch(out var bracketCloseToken, SyntaxKind.BracketCloseToken))
            {
                return new ArrayTypeSyntax(elementType, bracketOpenToken, bracketCloseToken);
            }

            var length = ParseExpression(iterator, allowUnterminated: false);
            bracketCloseToken = iterator.Match(SyntaxKind.BracketCloseToken);

            return new ArrayTypeSyntax(elementType, bracketOpenToken, length, bracketCloseToken);
        }

        static PointerTypeSyntax ParsePointerType(SyntaxIterator iterator, TypeSyntax elementType)
        {
            var starToken = iterator.Match(SyntaxKind.StarToken);

            return new PointerTypeSyntax(elementType, starToken);
        }

        static MaybeTypeSyntax ParseMaybeType(SyntaxIterator iterator, TypeSyntax underlyingType)
        {
            var hookToken = iterator.Match(SyntaxKind.HookToken);
            return new MaybeTypeSyntax(underlyingType, hookToken);
        }
    }
}
