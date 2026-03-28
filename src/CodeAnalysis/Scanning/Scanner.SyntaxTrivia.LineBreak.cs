using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    private static int ScanLineBreak(SourceText sourceText, int offset, out SyntaxTrivia trivia)
    {
        var read = 0;
        if (sourceText[(offset + read)..] is ['\r', '\n', ..])
            read++;
        read++;

        trivia = new SyntaxTrivia(SyntaxKind.LineBreakTrivia, new SourceSpan(sourceText, offset..(offset + read)));
        return read;
    }
}
