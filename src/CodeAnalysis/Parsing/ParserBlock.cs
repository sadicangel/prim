using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Statements;

namespace CodeAnalysis.Parsing;

internal static class ParserBlock
{
    extension(SyntaxTokenStream stream)
    {
        public BlockStatementSyntax ParseBlockStatement()
        {
            var braceOpenToken = stream.Match(SyntaxKind.BraceOpenToken);
            var statements = stream.ParseSyntaxList(
                [SyntaxKind.BraceCloseToken],
                ParseBlockExpressionStatement);
            var braceCloseToken = stream.Match(SyntaxKind.BraceCloseToken);

            return new BlockStatementSyntax(braceOpenToken, statements, braceCloseToken);
        }

        private StatementSyntax ParseBlockExpressionStatement() => stream.Current.SyntaxKind switch
        {
            SyntaxKind.LetKeyword or SyntaxKind.VarKeyword => stream.ParseLocalDeclaration(),
            _ => stream.ParseStatement(),
        };
    }
}
