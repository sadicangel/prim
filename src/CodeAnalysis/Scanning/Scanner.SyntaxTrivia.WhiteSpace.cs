using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    private static int ScanWhiteSpace(SourceText sourceText, int position, out SyntaxTrivia trivia)
    {
        var done = false;
        var read = 0;
        while (!done)
        {
            switch (sourceText[(position + read)..])
            {
                case []:
                case ['\0' or '\r' or '\n', ..]:
                case [var c, ..] when !char.IsWhiteSpace(c):
                    done = true;
                    break;
                default:
                    read++;
                    break;
            }
        }

        trivia = new SyntaxTrivia(SyntaxKind.WhiteSpaceTrivia, new SourceSpan(sourceText, position..(position + read)));
        return read;
    }
}
