using System.Globalization;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    private static int ScanNumber(SourceText sourceText, DiagnosticBag diagnostics, int offset, out SyntaxKind kind, out Range range, out object? value)
    {
        var read = 0;
        var isFloat = false;
        var isInvalid = false;
        var numberStyles = NumberStyles.Number;

        // TODO: Handle negative number literals.
        if (sourceText[offset + read] is '-')
            read++;

        switch (sourceText[(offset + read)..])
        {
            case ['0', 'b', ..]:
                {
                    read += 2;
                    while (sourceText[offset + read] is '0' or '1')
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
                    while (char.IsAsciiHexDigit(sourceText[offset + read]))
                    {
                        ++read;
                    }

                    numberStyles = NumberStyles.HexNumber;
                }
                break;
            default:
                {
                    while (char.IsAsciiDigit(sourceText[offset + read]))
                    {
                        ++read;
                    }

                    if (sourceText[offset + read] is '.')
                    {
                        isFloat = true;
                        ++read;
                        while (char.IsAsciiDigit(sourceText[offset + read]))
                        {
                            ++read;
                        }
                    }

                    if (sourceText[offset + read] is 'e' or 'E')
                    {
                        isFloat = true;
                        if (!char.IsAsciiDigit(sourceText[offset + read - 1]))
                        {
                            isInvalid = true;
                        }

                        ++read;
                        while (char.IsAsciiDigit(sourceText[offset + read]))
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
                switch (sourceText[(offset + read)..])
                {
                    case ['f', '1', '6', ..]:
                        read += 3;
                        kind = SyntaxKind.F16LiteralToken;
                        isInvalid = !Half.TryParse(sourceText[offset..(offset + read - 3)], out var f16);
                        value = f16;
                        break;

                    case ['f', '3', '2', ..]:
                        read += 3;
                        kind = SyntaxKind.F32LiteralToken;
                        isInvalid = !float.TryParse(sourceText[offset..(offset + read - 3)], out var f32);
                        value = f32;
                        break;

                    case ['f', '6', '4', ..]:
                        read += 3;
                        kind = SyntaxKind.F64LiteralToken;
                        isInvalid = !double.TryParse(sourceText[offset..(offset + read - 3)], out var f64);
                        value = f64;
                        break;

                    default:
                        kind = SyntaxKind.F64LiteralToken;
                        isInvalid = !double.TryParse(sourceText[offset..(offset + read)], out var @float);
                        value = @float;
                        break;
                }
            }
        }
        else
        {
            switch (sourceText[(offset + read)..])
            {
                case ['i', '8', ..]:
                    read += 2;
                    kind = SyntaxKind.I8LiteralToken;
                    isInvalid = !sbyte.TryParse(sourceText[offset..(offset + read - 2)], numberStyles, CultureInfo.InvariantCulture, out var i8);
                    value = i8;
                    break;

                case ['u', '8', ..]:
                    read += 2;
                    kind = SyntaxKind.U8LiteralToken;
                    isInvalid = !byte.TryParse(sourceText[offset..(offset + read - 2)], numberStyles, CultureInfo.InvariantCulture, out var u8);
                    value = u8;
                    break;

                case ['i', '1', '6', ..]:
                    read += 3;
                    kind = SyntaxKind.I16LiteralToken;
                    isInvalid = !short.TryParse(sourceText[offset..(offset + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var i16);
                    value = i16;
                    break;

                case ['u', '1', '6', ..]:
                    read += 3;
                    kind = SyntaxKind.U16LiteralToken;
                    isInvalid = !ushort.TryParse(sourceText[offset..(offset + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var u16);
                    value = u16;
                    break;

                case ['f', '1', '6', ..]:
                    read += 3;
                    kind = SyntaxKind.F16LiteralToken;
                    isInvalid = !Half.TryParse(sourceText[offset..(offset + read - 3)], CultureInfo.InvariantCulture, out var f16);
                    value = f16;
                    break;

                case ['i', '3', '2', ..]:
                    read += 3;
                    kind = SyntaxKind.I32LiteralToken;
                    isInvalid = !int.TryParse(sourceText[offset..(offset + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var i32);
                    value = i32;
                    break;

                case ['u', '3', '2', ..]:
                    read += 3;
                    kind = SyntaxKind.U32LiteralToken;
                    isInvalid = !uint.TryParse(sourceText[offset..(offset + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var u32);
                    value = u32;
                    break;

                case ['f', '3', '2', ..]:
                    read += 3;
                    kind = SyntaxKind.F32LiteralToken;
                    isInvalid = !float.TryParse(sourceText[offset..(offset + read - 3)], CultureInfo.InvariantCulture, out var f32);
                    value = f32;
                    break;

                case ['i', '6', '4', ..]:
                    read += 3;
                    kind = SyntaxKind.I64LiteralToken;
                    isInvalid = !long.TryParse(sourceText[offset..(offset + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var i64);
                    value = i64;
                    break;

                case ['u', '6', '4', ..]:
                    read += 3;
                    kind = SyntaxKind.U64LiteralToken;
                    isInvalid = !ulong.TryParse(sourceText[offset..(offset + read - 3)], numberStyles, CultureInfo.InvariantCulture, out var u64);
                    value = u64;
                    break;

                case ['f', '6', '4', ..]:
                    read += 3;
                    kind = SyntaxKind.F64LiteralToken;
                    isInvalid = !double.TryParse(sourceText[offset..(offset + read - 3)], CultureInfo.InvariantCulture, out var f64);
                    value = f64;
                    break;

                default:
                    kind = SyntaxKind.I32LiteralToken;
                    value = 0;
                    isInvalid = true;
                    if (long.TryParse(sourceText[offset..(offset + read)], numberStyles, CultureInfo.InvariantCulture, out var @int))
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

        range = offset..(offset + read);
        if (isInvalid)
        {
            diagnostics.ReportInvalidSyntaxValue(
                new SourceSpan(sourceText, range),
                kind);
        }

        return read;
    }
}
