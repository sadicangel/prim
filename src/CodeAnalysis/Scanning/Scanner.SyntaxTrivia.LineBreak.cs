using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;
partial class Scanner
{
    private static int ScanLineBreak(SyntaxTree syntaxTree, int offset, out SyntaxTrivia trivia)
    {
        var read = 0;
        if (syntaxTree.SourceText[(offset + read)..] is ['\r', '\n', ..])
            read++;
        read++;

        trivia = new SyntaxTrivia(SyntaxKind.LineBreakTrivia, syntaxTree, new SourceSpan(syntaxTree.SourceText, offset..(offset + read)));
        return read;
    }
}
