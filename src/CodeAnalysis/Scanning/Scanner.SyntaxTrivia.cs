using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    private static int ScanSyntaxTrivia(SourceText sourceText, DiagnosticBag diagnostics, int offset, bool leading, out SyntaxList<SyntaxTrivia> trivia)
    {
        var builder = ImmutableArray.CreateBuilder<SyntaxTrivia>();
        var length = 0;
        while (true)
        {
            var item = default(SyntaxTrivia)!;
            var read = sourceText[(offset + length)..] switch
            {
                ['/', '*', ..] => ScanMultiLineComment(sourceText, diagnostics, offset + length, out item),
                ['/', '/', ..] => ScanSingleLineComment(sourceText, offset + length, out item),
                ['\n' or '\r', ..] => ScanLineBreak(sourceText, offset + length, out item),
                [' ' or '\t', ..] => ScanWhiteSpace(sourceText, offset + length, out item),
                [var whitespace, ..] when char.IsWhiteSpace(whitespace) => ScanWhiteSpace(sourceText, offset + length, out item),
                _ => 0
            };

            if (read == 0)
                break;

            length += read;

            builder.Add(item);

            if (item.SyntaxKind == SyntaxKind.LineBreakTrivia && !leading)
                break;
        }

        trivia = new SyntaxList<SyntaxTrivia>(builder.ToImmutable());

        return length;
    }
}
