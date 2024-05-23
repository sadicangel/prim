using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParsePrimaryExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var current = iterator.Current;
        var next = iterator.Peek(1).SyntaxKind;
        return current.SyntaxKind switch
        {
            SyntaxKind.TrueKeyword =>
                new LiteralExpressionSyntax(SyntaxKind.TrueLiteralExpression, syntaxTree, current, PredefinedTypes.Bool, true),
            SyntaxKind.FalseKeyword =>
                new LiteralExpressionSyntax(SyntaxKind.FalseLiteralExpression, syntaxTree, current, PredefinedTypes.Bool, false),
            SyntaxKind.NullKeyword =>
                new LiteralExpressionSyntax(SyntaxKind.NullLiteralExpression, syntaxTree, current, PredefinedTypes.Unit, null),
            SyntaxKind.I32LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.I32LiteralExpression, syntaxTree, current, PredefinedTypes.I32, current.Value),
            SyntaxKind.U32LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.U32LiteralExpression, syntaxTree, current, PredefinedTypes.U32, current.Value),
            SyntaxKind.I64LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.I64LiteralExpression, syntaxTree, current, PredefinedTypes.I64, current.Value),
            SyntaxKind.U64LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.U64LiteralExpression, syntaxTree, current, PredefinedTypes.U64, current.Value),
            SyntaxKind.F32LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.F32LiteralExpression, syntaxTree, current, PredefinedTypes.F32, current.Value),
            SyntaxKind.F64LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.F64LiteralExpression, syntaxTree, current, PredefinedTypes.F64, current.Value),
            SyntaxKind.StrLiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.StrLiteralExpression, syntaxTree, current, PredefinedTypes.Str, current.Value),
            SyntaxKind.ParenthesisOpenToken =>
                ParseGroupExpression(syntaxTree, iterator),
            _ => next switch
            {
                SyntaxKind.ColonToken => ParseLocalDeclaration(syntaxTree, iterator),
                _ when SyntaxFacts.IsAssignmentOperator(next) => ParseAssignmentExpression(syntaxTree, iterator),
                _ => ParseIdentifierNameExpression(syntaxTree, iterator),
            }
        };
    }
}
