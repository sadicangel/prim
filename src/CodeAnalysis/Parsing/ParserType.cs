using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;

internal static class ParserType
{
    extension(SyntaxTokenStream stream)
    {
        public TypeClauseSyntax? ParseTypeClause([DoesNotReturnIf(false)] bool isOptional)
        {
            if (isOptional && stream.Current.SyntaxKind is not SyntaxKind.ColonToken)
            {
                return null;
            }

            var colonToken = stream.Match(SyntaxKind.ColonToken);
            var type = stream.ParseType();

            return new TypeClauseSyntax(colonToken, type);
        }

        public TypeSyntax ParseType()
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

            var types = stream.ParseSyntaxList(
                SyntaxKind.BarToken,
                [SyntaxKind.ColonToken, SyntaxKind.EqualsToken],
                ParseSingleType);

            return types is [var type] ? type : new UnionTypeSyntax(types);
        }

        private TypeSyntax ParseSingleType()
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
                    SyntaxKind.F64Keyword => stream.ParsePredefinedType(),

                SyntaxKind.ParenthesisOpenToken => stream.ParseLambdaType(),

                _ => stream.ParseNamedType()
            };

            return stream.Current.SyntaxKind switch
            {
                SyntaxKind.BracketOpenToken => stream.ParseArrayType(type),
                SyntaxKind.AsteriskToken => stream.ParsePointerType(type),
                SyntaxKind.HookToken => stream.ParseMaybeType(type),
                _ => type,
            };
        }

        private PredefinedTypeSyntax ParsePredefinedType()
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

        private LambdaTypeSyntax ParseLambdaType()
        {
            var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            var parameters = stream.ParseSyntaxList(
                SyntaxKind.CommaToken,
                [SyntaxKind.ParenthesisCloseToken, SyntaxKind.EofToken],
                ParseType);
            var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
            var arrowToken = stream.Match(SyntaxKind.MinusGreaterThanToken);
            var returnType = stream.ParseType();
            return new LambdaTypeSyntax(parenthesisOpenToken, parameters, parenthesisCloseToken, arrowToken, returnType);
        }

        private NamedTypeSyntax ParseNamedType()
        {
            var name = stream.ParseName();
            return new NamedTypeSyntax(name);
        }

        private ArrayTypeSyntax ParseArrayType(TypeSyntax elementType)
        {
            var bracketOpenToken = stream.Match(SyntaxKind.BracketOpenToken);
            if (stream.TryMatch(out var bracketCloseToken, SyntaxKind.BracketCloseToken))
            {
                return new ArrayTypeSyntax(elementType, bracketOpenToken, bracketCloseToken);
            }

            var length = stream.ParseExpressionTerminated();
            bracketCloseToken = stream.Match(SyntaxKind.BracketCloseToken);

            return new ArrayTypeSyntax(elementType, bracketOpenToken, length, bracketCloseToken);
        }

        private PointerTypeSyntax ParsePointerType(TypeSyntax elementType)
        {
            var starToken = stream.Match(SyntaxKind.AsteriskToken);

            return new PointerTypeSyntax(elementType, starToken);
        }

        private MaybeTypeSyntax ParseMaybeType(TypeSyntax underlyingType)
        {
            var hookToken = stream.Match(SyntaxKind.HookToken);
            return new MaybeTypeSyntax(underlyingType, hookToken);
        }
    }
}
