using System.Text;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    // TODO: Support raw strings.
    private static int ScanString(SourceText sourceText, DiagnosticBag diagnostics, int offset, out SyntaxKind kind, out Range range, out object? value)
    {
        var builder = new StringBuilder();
        var done = false;
        var read = 1;
        while (!done)
        {
            var span = sourceText[(offset + read)..];
            switch (span)
            {
                case ['\0', ..]:
                case ['\r', ..]:
                case ['\n', ..]:
                case []:
                    diagnostics.ReportUnterminatedString(new SourceSpan(sourceText, offset..(offset + 1)));
                    done = true;
                    break;
                case ['\\', '"', ..]:
                    read++;
                    builder.Append(span[1]);
                    read++;
                    break;
                case ['"', ..]:
                    read++;
                    done = true;
                    break;
                default:
                    builder.Append(span[0]);
                    read++;
                    break;
            }
        }

        kind = SyntaxKind.StrLiteralToken;
        range = offset..(offset + read);
        value = builder.ToString();
        return read;
    }
}
