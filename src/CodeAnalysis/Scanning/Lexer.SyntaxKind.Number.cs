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

        // TODO: Handle negative number literals.
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
                switch (syntaxTree.SourceText[(position + read)..])
                {
                    case ['f', '1', '6', ..]:
                        read += 3;
                        kind = SyntaxKind.F16LiteralToken;
                        isInvalid = !Half.TryParse(syntaxTree.SourceText[position..(position + read - 3)], out var f16);
                        value = f16;
                        break;

                    case ['f', '3', '2', ..]:
                        read += 3;
                        kind = SyntaxKind.F32LiteralToken;
                        isInvalid = !float.TryParse(syntaxTree.SourceText[position..(position + read - 3)], out var f32);
                        value = f32;
                        break;

                    case ['f', '6', '4', ..]:
                        read += 3;
                        kind = SyntaxKind.F64LiteralToken;
                        isInvalid = !double.TryParse(syntaxTree.SourceText[position..(position + read - 3)], out var f64);
                        value = f64;
                        break;

                    default:
                        kind = SyntaxKind.F64LiteralToken;
                        isInvalid = !double.TryParse(syntaxTree.SourceText[position..(position + read)], out var @float);
                        value = @float;
                        break;
                }
            }
        }
        else
        {
            switch (syntaxTree.SourceText[(position + read)..])
            {
                case ['i', '8', ..]:
                    read += 2;
                    kind = SyntaxKind.I8LiteralToken;
                    isInvalid = !sbyte.TryParse(syntaxTree.SourceText[position..(position + read - 2)], numberStyles, CultureInfo.InvariantCulture, out var i8);
                    value = i8;
                    break;

                case ['u', '8', ..]:
                    read += 2;
                    kind = SyntaxKind.U8LiteralToken;
                    isInvalid = !byte.TryParse(syntaxTree.SourceText[position..(position + read - 2)], numberStyles, CultureInfo.InvariantCulture, out var u8);
                    value = u8;
                    break;

                case ['i', '1', '6', ..]:
                    read += 3;
                    kind = SyntaxKind.I16LiteralToken;
                    isInvalid = !short.TryParse(syntaxTree.SourceText[position..(position + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var i16);
                    value = i16;
                    break;

                case ['u', '1', '6', ..]:
                    read += 3;
                    kind = SyntaxKind.U16LiteralToken;
                    isInvalid = !ushort.TryParse(syntaxTree.SourceText[position..(position + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var u16);
                    value = u16;
                    break;

                case ['f', '1', '6', ..]:
                    read += 3;
                    kind = SyntaxKind.F16LiteralToken;
                    isInvalid = !Half.TryParse(syntaxTree.SourceText[position..(position + read - 3)], CultureInfo.InvariantCulture, out var f16);
                    value = f16;
                    break;

                case ['i', '3', '2', ..]:
                    read += 3;
                    kind = SyntaxKind.I32LiteralToken;
                    isInvalid = !int.TryParse(syntaxTree.SourceText[position..(position + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var i32);
                    value = i32;
                    break;

                case ['u', '3', '2', ..]:
                    read += 3;
                    kind = SyntaxKind.U32LiteralToken;
                    isInvalid = !uint.TryParse(syntaxTree.SourceText[position..(position + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var u32);
                    value = u32;
                    break;

                case ['f', '3', '2', ..]:
                    read += 3;
                    kind = SyntaxKind.F32LiteralToken;
                    isInvalid = !float.TryParse(syntaxTree.SourceText[position..(position + read - 3)], CultureInfo.InvariantCulture, out var f32);
                    value = f32;
                    break;

                case ['i', '6', '4', ..]:
                    read += 3;
                    kind = SyntaxKind.I64LiteralToken;
                    isInvalid = !long.TryParse(syntaxTree.SourceText[position..(position + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var i64);
                    value = i64;
                    break;

                case ['u', '6', '4', ..]:
                    read += 3;
                    kind = SyntaxKind.U64LiteralToken;
                    isInvalid = !ulong.TryParse(syntaxTree.SourceText[position..(position + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var u64);
                    value = u64;
                    break;

                case ['f', '6', '4', ..]:
                    read += 3;
                    kind = SyntaxKind.F64LiteralToken;
                    isInvalid = !double.TryParse(syntaxTree.SourceText[position..(position + read - 3)], CultureInfo.InvariantCulture, out var f64);
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
