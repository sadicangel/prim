using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;
partial class Scanner
{
    private static int ScanWhiteSpace(SyntaxTree syntaxTree, int position, out SyntaxTrivia trivia)
    {
        var done = false;
        var read = 0;
        while (!done)
        {
            switch (syntaxTree.SourceText[(position + read)..])
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

        trivia = new SyntaxTrivia(SyntaxKind.WhiteSpaceTrivia, syntaxTree, new SourceSpan(syntaxTree.SourceText, position..(position + read)));
        return read;
    }
}
