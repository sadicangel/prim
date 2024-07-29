using System.Text;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;
partial class Lexer
{
    // TODO: Support raw strings.
    private static int ScanString(SyntaxTree syntaxTree, int position, out SyntaxKind kind, out Range range, out object? value)
    {
        var builder = new StringBuilder();
        var done = false;
        var read = 1;
        while (!done)
        {
            var span = syntaxTree.SourceText[(position + read)..];
            switch (span)
            {
                case ['\0', ..]:
                case ['\r', ..]:
                case ['\n', ..]:
                case []:
                    syntaxTree.Diagnostics.ReportUnterminatedString(new SourceLocation(syntaxTree.SourceText, position..(position + 1)));
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
        range = position..(position + read);
        value = builder.ToString();
        return read;
    }
}
