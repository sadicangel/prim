using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static TypeSyntax ParseType(SyntaxTree syntaxTree, SyntaxIterator iterator)
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
            syntaxTree,
            iterator,
            SyntaxKind.PipeToken,
            [SyntaxKind.ColonToken, SyntaxKind.EqualsToken],
            ParseSingleType);

        return types is [var type] ? type : new UnionTypeSyntax(syntaxTree, types);

        static TypeSyntax ParseSingleType(SyntaxTree syntaxTree, SyntaxIterator iterator)
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
                SyntaxKind.F64Keyword => ParsePredefinedType(syntaxTree, iterator),

                SyntaxKind.ParenthesisOpenToken => ParseLambdaType(syntaxTree, iterator),

                _ => ParseNamedType(syntaxTree, iterator)
            };

            return iterator.Current.SyntaxKind switch
            {
                SyntaxKind.BracketOpenToken => ParseArrayType(syntaxTree, iterator, type),
                SyntaxKind.StarToken => ParsePointerType(syntaxTree, iterator, type),
                SyntaxKind.HookToken => ParseMaybeType(syntaxTree, iterator, type),
                _ => type,
            };
        }

        static PredefinedTypeSyntax ParsePredefinedType(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var predefinedTypeToken = iterator.Match([
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

            return new PredefinedTypeSyntax(syntaxTree, predefinedTypeToken);
        }

        static LambdaTypeSyntax ParseLambdaType(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
            var parameters = ParseSyntaxList(
                syntaxTree,
                iterator,
                SyntaxKind.CommaToken,
                [SyntaxKind.ParenthesisCloseToken, SyntaxKind.EofToken],
                ParseType);
            var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
            var arrowToken = iterator.Match(SyntaxKind.ArrowReturnToken);
            var returnType = ParseType(syntaxTree, iterator);
            return new LambdaTypeSyntax(syntaxTree, parenthesisOpenToken, parameters, parenthesisCloseToken, arrowToken, returnType);
        }

        static NamedTypeSyntax ParseNamedType(SyntaxTree syntaxTree, SyntaxIterator iterator)
        {
            var name = ParseName(syntaxTree, iterator);
            return new NamedTypeSyntax(syntaxTree, name);
        }

        static ArrayTypeSyntax ParseArrayType(SyntaxTree syntaxTree, SyntaxIterator iterator, TypeSyntax elementType)
        {
            var bracketOpenToken = iterator.Match(SyntaxKind.BracketOpenToken);
            var length = ParseExpression(syntaxTree, iterator, allowUnterminated: false);
            var bracketCloseToken = iterator.Match(SyntaxKind.BracketCloseToken);

            return new ArrayTypeSyntax(syntaxTree, elementType, bracketOpenToken, length, bracketCloseToken);
        }

        static PointerTypeSyntax ParsePointerType(SyntaxTree syntaxTree, SyntaxIterator iterator, TypeSyntax elementType)
        {
            var starToken = iterator.Match(SyntaxKind.StarToken);

            return new PointerTypeSyntax(syntaxTree, elementType, starToken);
        }

        static MaybeTypeSyntax ParseMaybeType(SyntaxTree syntaxTree, SyntaxIterator iterator, TypeSyntax underlyingType)
        {
            var hookToken = iterator.Match(SyntaxKind.HookToken);
            return new MaybeTypeSyntax(syntaxTree, underlyingType, hookToken);
        }
    }
}
