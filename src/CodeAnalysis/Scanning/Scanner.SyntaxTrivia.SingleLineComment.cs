using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    private static int ScanSingleLineComment(SourceText sourceText, int position, out SyntaxTrivia trivia)
    {
        // Skip '//'.
        var read = 2;
        while (sourceText[position + read] is not '\r' and not '\n' and not '\0')
            read++;

        trivia = new SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, new SourceSpan(sourceText, position..(position + read)));
        return read;
    }
}
