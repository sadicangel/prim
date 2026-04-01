using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static TypeSyntax ParseType(SyntaxTokenStream stream)
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
            stream,
            SyntaxKind.PipeToken,
            [SyntaxKind.ColonToken, SyntaxKind.EqualsToken],
            ParseSingleType);

        return types is [var type] ? type : new UnionTypeSyntax(types);

        static TypeSyntax ParseSingleType(SyntaxTokenStream stream)
        {
            TypeSyntax type = stream.Current.SyntaxKind switch
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
                    SyntaxKind.F64Keyword => ParsePredefinedType(stream),

                SyntaxKind.ParenthesisOpenToken => ParseLambdaType(stream),

                _ => ParseNamedType(stream)
            };

            return stream.Current.SyntaxKind switch
            {
                SyntaxKind.BracketOpenToken => ParseArrayType(stream, type),
                SyntaxKind.StarToken => ParsePointerType(stream, type),
                SyntaxKind.HookToken => ParseMaybeType(stream, type),
                _ => type,
            };
        }

        static PredefinedTypeSyntax ParsePredefinedType(SyntaxTokenStream stream)
        {
            var predefinedTypeToken = stream.Match(
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

        static LambdaTypeSyntax ParseLambdaType(SyntaxTokenStream stream)
        {
            var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            var parameters = ParseSyntaxList(
                stream,
                SyntaxKind.CommaToken,
                [SyntaxKind.ParenthesisCloseToken, SyntaxKind.EofToken],
                ParseType);
            var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
            var arrowToken = stream.Match(SyntaxKind.MinusGreaterThanToken);
            var returnType = ParseType(stream);
            return new LambdaTypeSyntax(parenthesisOpenToken, parameters, parenthesisCloseToken, arrowToken, returnType);
        }

        static NamedTypeSyntax ParseNamedType(SyntaxTokenStream stream)
        {
            var name = ParseName(stream);
            return new NamedTypeSyntax(name);
        }

        static ArrayTypeSyntax ParseArrayType(SyntaxTokenStream stream, TypeSyntax elementType)
        {
            var bracketOpenToken = stream.Match(SyntaxKind.BracketOpenToken);
            if (stream.TryMatch(out var bracketCloseToken, SyntaxKind.BracketCloseToken))
            {
                return new ArrayTypeSyntax(elementType, bracketOpenToken, bracketCloseToken);
            }

            var length = ParseExpression(stream, allowUnterminated: false);
            bracketCloseToken = stream.Match(SyntaxKind.BracketCloseToken);

            return new ArrayTypeSyntax(elementType, bracketOpenToken, length, bracketCloseToken);
        }

        static PointerTypeSyntax ParsePointerType(SyntaxTokenStream stream, TypeSyntax elementType)
        {
            var starToken = stream.Match(SyntaxKind.StarToken);

            return new PointerTypeSyntax(elementType, starToken);
        }

        static MaybeTypeSyntax ParseMaybeType(SyntaxTokenStream stream, TypeSyntax underlyingType)
        {
            var hookToken = stream.Match(SyntaxKind.HookToken);
            return new MaybeTypeSyntax(underlyingType, hookToken);
        }
    }
}
