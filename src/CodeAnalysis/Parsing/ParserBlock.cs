using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal static class ParserBlock
{
    extension(SyntaxTokenStream stream)
    {
        public BlockExpressionSyntax ParseBlockExpression()
        {
            var braceOpenToken = stream.Match(SyntaxKind.BraceOpenToken);
            var expressions = stream.ParseSyntaxList(
                [SyntaxKind.BraceCloseToken],
                ParseBlockExpressionStatement);
            var braceCloseToken = stream.Match(SyntaxKind.BraceCloseToken);

            return new BlockExpressionSyntax(braceOpenToken, expressions, braceCloseToken);
        }

        private ExpressionSyntax ParseBlockExpressionStatement() => stream.Current.SyntaxKind switch
        {
            SyntaxKind.LetKeyword or SyntaxKind.VarKeyword => stream.ParseLocalDeclaration(),
            _ => stream.ParseExpressionTerminated(),
        };
    }
}
