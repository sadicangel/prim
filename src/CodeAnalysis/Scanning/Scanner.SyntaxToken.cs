using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    private static int ScanSyntaxToken(SourceText sourceText, DiagnosticBag diagnostics, int offset, out SyntaxToken token)
    {
        var read = 0;
        read += ScanSyntaxTrivia(sourceText, diagnostics, offset + read, leading: true, out var leadingTrivia);
        read += ScanSyntaxKind(sourceText, diagnostics, offset + read, out var syntaxKind, out var range, out var value);
        read += ScanSyntaxTrivia(sourceText, diagnostics, offset + read, leading: false, out var trailingTrivia);

        var sourceSpan = new SourceSpan(sourceText, range);

        token = new SyntaxToken(syntaxKind, sourceSpan, leadingTrivia, trailingTrivia, value);
        return read;
    }
}
