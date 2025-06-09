using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;
partial class Scanner
{
    private static int ScanMultiLineComment(SyntaxTree syntaxTree, DiagnosticBag diagnostics, int offset, out SyntaxTrivia trivia)
    {
        var done = false;
        // Skip '/*'.
        var read = 2;
        while (!done)
        {
            switch (syntaxTree.SourceText[(offset + read)..])
            {
                case []:
                case ['\0', ..]:
                    diagnostics.ReportUnterminatedComment(new SourceSpan(syntaxTree.SourceText, (offset + read)..(offset + read + 2)));
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

        trivia = new SyntaxTrivia(SyntaxKind.MultiLineCommentTrivia, syntaxTree, new SourceSpan(syntaxTree.SourceText, offset..(offset + read)));
        return read;
    }
}
