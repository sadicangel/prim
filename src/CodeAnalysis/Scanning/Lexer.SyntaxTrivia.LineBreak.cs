using CodeAnalysis.Syntax;

namespace CodeAnalysis.Scanning;
partial class Lexer
{
    private static int ScanLineBreak(SyntaxTree syntaxTree, int position, out SyntaxTrivia trivia)
    {
        var read = 0;
        if (syntaxTree.SourceText[(position + read)..] is ['\r', '\n', ..])
            read++;
        read++;

        trivia = new SyntaxTrivia(SyntaxKind.LineBreakTrivia, syntaxTree, position..(position + read));
        return read;
    }
}
