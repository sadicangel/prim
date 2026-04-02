using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;

internal static class ParserExpressionCore
{
    extension(SyntaxTokenStream stream)
    {
        // binary_expression = unary-expression (binary-operator binary_expression)*
        public ExpressionSyntax ParseBinaryExpression(int? parentPrecedence = null)
        {
            var left = stream.ParseUnaryExpression(parentPrecedence);

begin:
            switch (stream.Current.SyntaxKind)
            {
                //case SyntaxKind.AsKeyword:
                //    {
                //        var asKeyword = stream.Match(SyntaxKind.AsKeyword);
                //        var type = ParseType(syntaxTree, stream);
                //        left = new ConversionExpressionSyntax(syntaxTree, left, asKeyword, type);
                //    }
                //    goto begin;

                //case SyntaxKind.DotToken:
                //    {
                //        var dotToken = stream.Match(SyntaxKind.DotToken);
                //        var name = ParseSimpleName(syntaxTree, stream);
                //        left = new MemberAccessExpressionSyntax(syntaxTree, left, dotToken, name);
                //    }
                //    goto begin;

                //case SyntaxKind.BraceOpenToken when left is NameSyntax name:
                //    {
                //        var braceOpenToken = stream.Match(SyntaxKind.BraceOpenToken);
                //        var properties = ParseSeparatedSyntaxList(
                //            syntaxTree,
                //            stream,
                //            SyntaxKind.CommaToken,
                //            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
                //            ParsePropertyInitExpression);
                //        var braceCloseToken = stream.Match(SyntaxKind.BraceCloseToken);
                //        left = new StructInitExpressionSyntax(syntaxTree, name, braceOpenToken, properties, braceCloseToken);
                //    }
                //    goto begin;

                //case SyntaxKind.BracketOpenToken:
                //    {
                //        var bracketOpenToken = stream.Match(SyntaxKind.BracketOpenToken);
                //        var index = ParseExpression(syntaxTree, stream);
                //        var bracketCloseToken = stream.Match(SyntaxKind.BracketCloseToken);

                //        left = new IndexExpressionSyntax(syntaxTree, left, bracketOpenToken, index, bracketCloseToken);
                //    }
                //    goto begin;

                case SyntaxKind.ParenthesisOpenToken:
                    {
                        var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
                        var arguments = stream.ParseSyntaxList(
                            SyntaxKind.CommaToken,
                            [SyntaxKind.ParenthesisCloseToken, SyntaxKind.EofToken],
                            ParserExpression.ParseExpression);
                        var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);

                        left = new InvocationExpressionSyntax(left, parenthesisOpenToken, arguments, parenthesisCloseToken);
                    }
                    goto begin;

                case SyntaxKind.EqualsToken:
                    {
                        var equalsToken = stream.Match(SyntaxKind.EqualsToken);
                        var right = stream.ParseBinaryExpression();
                        left = new AssignmentExpressionSyntax(left, equalsToken, right);
                    }
                    goto begin;

                case { } when SyntaxFacts.GetBinaryOperatorPrecedence(stream.Current.SyntaxKind) is var binaryPrecedence && binaryPrecedence >= parentPrecedence.GetValueOrDefault():
                    {
                        var operatorToken = stream.Match();
                        var syntaxKind = SyntaxFacts.GetBinaryOperatorExpression(operatorToken.SyntaxKind);
                        var right = stream.ParseBinaryExpression(binaryPrecedence);
                        left = new BinaryExpressionSyntax(syntaxKind, left, operatorToken, right);
                    }
                    goto begin;
            }

            return left;
        }

        // unary_expression = unary_operator binary_expression | primary_expression
        private ExpressionSyntax ParseUnaryExpression(int? parentPrecedence = null)
        {
            var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(stream.Current.SyntaxKind);
            if (unaryPrecedence >= parentPrecedence.GetValueOrDefault())
            {
                var operatorToken = stream.Match();
                var syntaxKind = SyntaxFacts.GetUnaryOperatorExpression(operatorToken.SyntaxKind);
                var operand = stream.ParseBinaryExpression(unaryPrecedence);

                return new UnaryExpressionSyntax(syntaxKind, operatorToken, operand);
            }

            return stream.ParsePrimaryExpression();
        }

        // primary_expression = type | literal | group | name
        private ExpressionSyntax ParsePrimaryExpression()
        {
            var current = stream.Current;
            return current.SyntaxKind switch
            {
                >= SyntaxKind.AnyKeyword and <= SyntaxKind.F64Keyword => new SimpleNameSyntax(stream.Match()),
                >= SyntaxKind.TrueKeyword and <= SyntaxKind.NullKeyword => stream.ParseLiteralExpression(),
                >= SyntaxKind.I8LiteralToken and <= SyntaxKind.StrLiteralToken => stream.ParseLiteralExpression(),
                SyntaxKind.BracketOpenToken => stream.ParseArrayInitializerExpression(),
                SyntaxKind.ParenthesisOpenToken when stream.IsLambdaExpressionAhead() => stream.ParseLambdaExpression(),
                SyntaxKind.ParenthesisOpenToken => stream.ParseGroupExpression(),
                _ => stream.ParseName(),
            };
        }

        // literal_expression = 'true' | 'false' | 'null' | i8_literal | u8_literal | i16_literal | u16_literal | i32_literal | u32_literal | i64_literal | u64_literal | f16_literal | f32_literal | f64_literal | str_literal
        private LiteralExpressionSyntax ParseLiteralExpression()
        {
            var literalToken = stream.Match();

            return literalToken.SyntaxKind switch
            {
                SyntaxKind.TrueKeyword =>
                    new LiteralExpressionSyntax(SyntaxKind.TrueLiteralExpression, literalToken, true),
                SyntaxKind.FalseKeyword =>
                    new LiteralExpressionSyntax(SyntaxKind.FalseLiteralExpression, literalToken, false),
                SyntaxKind.NullKeyword =>
                    new LiteralExpressionSyntax(SyntaxKind.NullLiteralExpression, literalToken, Unit.Value),
                SyntaxKind.I8LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.I8LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.U8LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.U8LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.I16LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.I16LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.U16LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.U16LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.I32LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.I32LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.U32LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.U32LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.I64LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.I64LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.U64LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.U64LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.F16LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.F16LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.F32LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.F32LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.F64LiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.F64LiteralExpression, literalToken, literalToken.Value!),
                SyntaxKind.StrLiteralToken =>
                    new LiteralExpressionSyntax(SyntaxKind.StrLiteralExpression, literalToken, literalToken.Value!),
                _ =>
                    throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{literalToken.SyntaxKind}' for {nameof(LiteralExpressionSyntax)}")
            };
        }

        private ArrayInitializerExpressionSyntax ParseArrayInitializerExpression()
        {
            var bracketOpenToken = stream.Match(SyntaxKind.BracketOpenToken);
            var elements = stream.ParseSyntaxList(
                SyntaxKind.CommaToken,
                [SyntaxKind.BracketCloseToken],
                ParserExpression.ParseExpression);
            var bracketCloseToken = stream.Match(SyntaxKind.BracketCloseToken);

            return new ArrayInitializerExpressionSyntax(bracketOpenToken, elements, bracketCloseToken);
        }

        private LambdaExpressionSyntax ParseLambdaExpression()
        {
            var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            var parameters = stream.ParseSyntaxList(
                SyntaxKind.CommaToken,
                [SyntaxKind.ParenthesisCloseToken],
                ParserName.ParseSimpleName);

            var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
            var equalsGreaterThanToken = stream.Match(SyntaxKind.EqualsGreaterThanToken);
            // TODO: This should be block expression or statement expression. Not all.
            var body = stream.ParseExpressionTerminated();

            return new LambdaExpressionSyntax(
                parenthesisOpenToken,
                parameters,
                parenthesisCloseToken,
                equalsGreaterThanToken,
                body);
        }

        // group_expression = '(' expr ')'
        private GroupExpressionSyntax ParseGroupExpression()
        {
            var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            var expression = stream.ParseExpression();
            var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);

            return new GroupExpressionSyntax(
                parenthesisOpenToken,
                expression,
                parenthesisCloseToken);
        }
    }
}
