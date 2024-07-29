using CodeAnalysis.Syntax;

namespace CodeAnalysis.Scanning;
partial class Lexer
{
    private static int ScanSingleLineComment(SyntaxTree syntaxTree, int position, out SyntaxTrivia trivia)
    {
        // Skip '//'.
        var read = 2;
        while (syntaxTree.SourceText[position + read] is not '\r' and not '\n' and not '\0')
            read++;

        trivia = new SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, syntaxTree, position..(position + read));
        return read;
    }
}
