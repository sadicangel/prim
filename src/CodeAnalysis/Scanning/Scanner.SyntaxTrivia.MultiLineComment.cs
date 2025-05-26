using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;
partial class Scanner
{
    private static int ScanMultiLineComment(SyntaxTree syntaxTree, int position, out SyntaxTrivia trivia)
    {
        var done = false;
        // Skip '/*'.
        var read = 2;
        while (!done)
        {
            switch (syntaxTree.SourceText[(position + read)..])
            {
                case []:
                case ['\0', ..]:
                    syntaxTree.Diagnostics.ReportUnterminatedComment(new SourceSpan(syntaxTree.SourceText, (position + read)..(position + read + 2)));
                    done = true;
                    break;
                case ['*', '/', ..]:
                    read += 2;
                    done = true;
                    break;
                default:
                    read++;
                    break;
            }
        }

        trivia = new SyntaxTrivia(SyntaxKind.MultiLineCommentTrivia, syntaxTree, new SourceSpan(syntaxTree.SourceText, position..(position + read)));
        return read;
    }
}
