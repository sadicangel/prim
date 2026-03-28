using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // literal_expression = 'true' | 'false' | 'null' | i8_literal | u8_literal | i16_literal | u16_literal | i32_literal | u32_literal | i64_literal | u64_literal | f16_literal | f32_literal | f64_literal | str_literal
    public static LiteralExpressionSyntax ParseLiteralExpression(SyntaxIterator iterator)
    {
        var literalToken = iterator.Match();

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
}
