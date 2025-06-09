using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;
partial class Scanner
{
    private static int ScanSyntaxToken(SyntaxTree syntaxTree, DiagnosticBag diagnostics, int offset, out SyntaxToken token)
    {
        var read = 0;
        read += ScanSyntaxTrivia(syntaxTree, diagnostics, offset + read, leading: true, out var leadingTrivia);
        read += ScanSyntaxKind(syntaxTree, diagnostics, offset + read, out var syntaxKind, out var range, out var value);
        read += ScanSyntaxTrivia(syntaxTree, diagnostics, offset + read, leading: false, out var trailingTrivia);

        var span = new SourceSpan(syntaxTree.SourceText, range);

        token = new SyntaxToken(syntaxKind, syntaxTree, span, leadingTrivia, trailingTrivia, value);
        return read;
    }
}
