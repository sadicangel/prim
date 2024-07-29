using System.Globalization;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;
partial class Lexer
{
    private static int ScanNumber(SyntaxTree syntaxTree, int position, out SyntaxKind kind, out Range range, out object? value)
    {
        var read = 0;
        var isFloat = false;
        var isInvalid = false;
        var numberStyles = NumberStyles.Number;

        // TODO: Actually handle this case.
        if (syntaxTree.SourceText[position + read] is '-')
            read++;

        switch (syntaxTree.SourceText[(position + read)..])
        {
            case ['0', 'b', ..]:
                {
                    read += 2;
                    while (syntaxTree.SourceText[position + read] is '0' or '1')
                    {
                        ++read;
                    }
                    numberStyles = NumberStyles.BinaryNumber;
                }
                break;
            //case ['0', '0', ..]: Octal
            case ['0', 'x' or 'X', ..]:
                {
                    read += 2;
                    while (char.IsAsciiHexDigit(syntaxTree.SourceText[position + read]))
                    {
                        ++read;
                    }
                    numberStyles = NumberStyles.HexNumber;
                }
                break;
            default:
                {
                    while (char.IsAsciiDigit(syntaxTree.SourceText[position + read]))
                    {
                        ++read;
                    }

                    if (syntaxTree.SourceText[position + read] is '.')
                    {
                        isFloat = true;
                        ++read;
                        while (char.IsAsciiDigit(syntaxTree.SourceText[position + read]))
                        {
                            ++read;
                        }
                    }

                    if (syntaxTree.SourceText[position + read] is 'e' or 'E')
                    {
                        isFloat = true;
                        if (!char.IsAsciiDigit(syntaxTree.SourceText[position + read - 1]))
                        {
                            isInvalid = true;
                        }
                        ++read;
                        while (char.IsAsciiDigit(syntaxTree.SourceText[position + read]))
                        {
                            ++read;
                        }
                    }
                    break;
                }
        }

        if (isFloat)
        {
            kind = SyntaxKind.F64LiteralToken;
            value = 0D;

            if (!isInvalid)
            {
                switch (syntaxTree.SourceText[position + read])
                {
                    case 'f' or 'F':
                        read++;
                        kind = SyntaxKind.F32LiteralToken;
                        isInvalid = !float.TryParse(syntaxTree.SourceText[position..(position + read - 1)], out var f32);
                        value = f32;
                        break;

                    case 'd' or 'D':
                        read++;
                        kind = SyntaxKind.F64LiteralToken;
                        isInvalid = !double.TryParse(syntaxTree.SourceText[position..(position + read - 1)], out var f64);
                        value = f64;
                        break;

                    default:
                        kind = SyntaxKind.F64LiteralToken;
                        isInvalid = !double.TryParse(syntaxTree.SourceText[position..(position + read - 1)], out var @float);
                        value = @float;
                        break;
                }
            }
        }
        else
        {
            switch (syntaxTree.SourceText[(position + read)..])
            {
                case ['l' or 'L', ..]:
                    read++;
                    kind = SyntaxKind.I64LiteralToken;
                    isInvalid = !long.TryParse(syntaxTree.SourceText[position..(position + read - 1)], numberStyles, CultureInfo.InvariantCulture, out var i64);
                    value = i64;
                    break;

                case ['u' or 'U', 'l' or 'L', ..]:
                    read += 2;
                    kind = SyntaxKind.U64LiteralToken;
                    isInvalid = !ulong.TryParse(syntaxTree.SourceText[position..(position + read - 2)], numberStyles, CultureInfo.InvariantCulture, out var u64);
                    value = u64;
                    break;

                case ['u' or 'U', ..]:
                    read++;
                    kind = SyntaxKind.U32LiteralToken;
                    isInvalid = !uint.TryParse(syntaxTree.SourceText[position..(position + read - 1)], numberStyles, CultureInfo.InvariantCulture, out var u32);
                    value = u32;
                    break;

                case ['f' or 'F', ..]:
                    read++;
                    kind = SyntaxKind.F32LiteralToken;
                    isInvalid = !float.TryParse(syntaxTree.SourceText[position..(position + read - 1)], CultureInfo.InvariantCulture, out var f32);
                    value = f32;
                    break;

                case ['d' or 'D', ..]:
                    read++;
                    kind = SyntaxKind.F64LiteralToken;
                    isInvalid = !double.TryParse(syntaxTree.SourceText[position..(position + read - 1)], CultureInfo.InvariantCulture, out var f64);
                    value = f64;
                    break;

                default:
                    kind = SyntaxKind.I32LiteralToken;
                    value = 0;
                    isInvalid = true;
                    if (long.TryParse(syntaxTree.SourceText[position..(position + read)], numberStyles, CultureInfo.InvariantCulture, out var @int))
                    {
                        isInvalid = false;
                        if (@int is >= int.MinValue and <= int.MaxValue)
                        {
                            kind = SyntaxKind.I32LiteralToken;
                            value = (int)@int;
                        }
                        else
                        {
                            kind = SyntaxKind.I64LiteralToken;
                            value = @int;
                        }
                    }
                    break;
            }
        }

        range = position..(position + read);
        if (isInvalid)
        {
            syntaxTree.Diagnostics.ReportInvalidSyntaxValue(
                new SourceLocation(syntaxTree.SourceText, range),
                kind);
        }

        return read;
    }
}
