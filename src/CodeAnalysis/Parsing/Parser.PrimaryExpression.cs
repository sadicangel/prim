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
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.TrueLiteralExpression, PredefinedTypes.Bool, true),
            SyntaxKind.FalseKeyword =>
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.FalseLiteralExpression, PredefinedTypes.Bool, false),
            SyntaxKind.NullKeyword =>
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.NullLiteralExpression, PredefinedTypes.Unit, null),
            SyntaxKind.I32LiteralToken =>
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.I32LiteralExpression, PredefinedTypes.I32, current.Value),
            SyntaxKind.U32LiteralToken =>
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.U32LiteralExpression, PredefinedTypes.U32, current.Value),
            SyntaxKind.I64LiteralToken =>
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.I64LiteralExpression, PredefinedTypes.I64, current.Value),
            SyntaxKind.U64LiteralToken =>
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.U64LiteralExpression, PredefinedTypes.U64, current.Value),
            SyntaxKind.F32LiteralToken =>
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.F32LiteralExpression, PredefinedTypes.F32, current.Value),
            SyntaxKind.F64LiteralToken =>
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.F64LiteralExpression, PredefinedTypes.F64, current.Value),
            SyntaxKind.StrLiteralToken =>
                ParseLiteralExpression(syntaxTree, iterator, SyntaxKind.StrLiteralExpression, PredefinedTypes.Str, current.Value),
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
