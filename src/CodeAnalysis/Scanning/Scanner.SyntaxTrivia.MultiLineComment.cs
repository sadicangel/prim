using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    private static int ScanMultiLineComment(SourceText sourceText, DiagnosticBag diagnostics, int offset, out SyntaxTrivia trivia)
    {
        var done = false;
        // Skip '/*'.
        var read = 2;
        while (!done)
        {
            switch (sourceText[(offset + read)..])
            {
                case []:
                case ['\0', ..]:
                    diagnostics.ReportUnterminatedComment(new SourceSpan(sourceText, (offset + read)..(offset + read + 2)));
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

        trivia = new SyntaxTrivia(SyntaxKind.MultiLineCommentTrivia, new SourceSpan(sourceText, offset..(offset + read)));
        return read;
    }
}
