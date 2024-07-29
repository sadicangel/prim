using CodeAnalysis.Syntax;

namespace CodeAnalysis.Scanning;
partial class Lexer
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

        trivia = new SyntaxTrivia(SyntaxKind.WhiteSpaceTrivia, syntaxTree, position..(position + read));
        return read;
    }
}
