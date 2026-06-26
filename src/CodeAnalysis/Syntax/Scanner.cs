using System.Globalization;
using System.Text;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

internal sealed class Scanner(SourceText sourceText)
{
    private readonly DiagnosticBag _diagnostics = new();
    private int _position = 0;

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    public IEnumerable<SyntaxToken> ScanAll()
    {
        var invalidTokens = new List<SyntaxToken>();
        while (true)
        {
            var syntaxToken = ScanAny();
            if (syntaxToken.Kind is SyntaxKind.InvalidSyntaxToken)
            {
                invalidTokens.Add(syntaxToken);
                continue;
            }

            if (invalidTokens.Count > 0)
            {
                var trivia = new List<SyntaxTrivia>();
                foreach (var token in invalidTokens)
                {
                    trivia.AddRange(token.LeadingTrivia);
                    trivia.Add(new SyntaxTrivia(SyntaxKind.InvalidSyntaxTrivia, token.SourceSpan));
                    trivia.AddRange(token.TrailingTrivia);
                }

                trivia.AddRange(syntaxToken.LeadingTrivia);
                invalidTokens.Clear();

                syntaxToken = syntaxToken with { LeadingTrivia = [.. trivia] };
            }

            yield return syntaxToken;

            if (syntaxToken.Kind is SyntaxKind.EofToken)
                break;
        }
    }

    public SyntaxToken ScanAny()
    {
        var leadingTrivia = ScanTrivia();
        var (syntaxKind, sourceSpan, value) = ScanSyntaxKind();
        var trailingTrivia = ScanTrivia();
        return new SyntaxToken(syntaxKind, sourceSpan, leadingTrivia, trailingTrivia, value);
    }

    private (SyntaxKind, SourceSpan, object? Value) ScanSyntaxKind()
    {
        var start = _position;
        switch (sourceText[start..])
        {
            case ['{', ..]:
                _position += 1;
                return Token(SyntaxKind.BraceOpenToken, start);
            case ['}', ..]:
                _position += 1;
                return Token(SyntaxKind.BraceCloseToken, start);
            case ['(', ..]:
                _position += 1;
                return Token(SyntaxKind.ParenthesisOpenToken, start);
            case [')', ..]:
                _position += 1;
                return Token(SyntaxKind.ParenthesisCloseToken, start);
            case ['[', ..]:
                _position += 1;
                return Token(SyntaxKind.BracketOpenToken, start);
            case [']', ..]:
                _position += 1;
                return Token(SyntaxKind.BracketCloseToken, start);
            case [':', ..]:
                _position += 1;
                return Token(SyntaxKind.ColonToken, start);
            case [';', ..]:
                _position += 1;
                return Token(SyntaxKind.SemicolonToken, start);
            case [',', ..]:
                _position += 1;
                return Token(SyntaxKind.CommaToken, start);
            case ['=', '>', ..]:
                _position += 2;
                return Token(SyntaxKind.EqualsGreaterThanToken, start);
            case ['-', '>', ..]:
                _position += 2;
                return Token(SyntaxKind.MinusGreaterThanToken, start);
            case ['&', '&', ..]:
                _position += 2;
                return Token(SyntaxKind.AmpersandAmpersandToken, start);
            case ['&', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.AmpersandEqualsToken, start);
            case ['&', ..]:
                _position += 1;
                return Token(SyntaxKind.AmpersandToken, start);
            case ['!', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.ExclamationEqualsToken, start);
            case ['!', ..]:
                _position += 1;
                return Token(SyntaxKind.ExclamationToken, start);
            case ['.', '.', ..]:
                _position += 2;
                return Token(SyntaxKind.DotDotToken, start);
            case ['.']:
            case ['.', not (>= '0' and <= '9'), ..]:
                _position += 1;
                return Token(SyntaxKind.DotToken, start);
            case ['=', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.EqualsEqualsToken, start);
            case ['=', ..]:
                _position += 1;
                return Token(SyntaxKind.EqualsToken, start);
            case ['>', '>', '=', ..]:
                _position += 3;
                return Token(SyntaxKind.GreaterThanGreaterThanEqualsToken, start);
            case ['>', '>', ..]:
                _position += 2;
                return Token(SyntaxKind.GreaterThanGreaterThanToken, start);
            case ['>', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.GreaterThanEqualsToken, start);
            case ['>', ..]:
                _position += 1;
                return Token(SyntaxKind.GreaterThanToken, start);
            case ['^', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.CaretEqualsToken, start);
            case ['^', ..]:
                _position += 1;
                return Token(SyntaxKind.CaretToken, start);
            case ['?', '?', '=', ..]:
                _position += 3;
                return Token(SyntaxKind.HookHookEqualsToken, start);
            case ['?', '?', ..]:
                _position += 2;
                return Token(SyntaxKind.HookHookToken, start);
            case ['?', ..]:
                _position += 1;
                return Token(SyntaxKind.HookToken, start);
            case ['<', '<', '=', ..]:
                _position += 3;
                return Token(SyntaxKind.LessThanLessThanEqualsToken, start);
            case ['<', '<', ..]:
                _position += 2;
                return Token(SyntaxKind.LessThanLessThanToken, start);
            case ['<', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.LessThanEqualsToken, start);
            case ['<', ..]:
                _position += 1;
                return Token(SyntaxKind.LessThanToken, start);
            case ['-', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.MinusEqualsToken, start);
            case ['-', ..]:
                _position += 1;
                return Token(SyntaxKind.MinusToken, start);
            case ['%', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.PercentEqualsToken, start);
            case ['%', ..]:
                _position += 1;
                return Token(SyntaxKind.PercentToken, start);
            case ['|', '|', ..]:
                _position += 2;
                return Token(SyntaxKind.BarBarToken, start);
            case ['|', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.PipeEqualsToken, start);
            case ['|', ..]:
                _position += 1;
                return Token(SyntaxKind.BarToken, start);
            case ['+', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.PlusEqualsToken, start);
            case ['+', ..]:
                _position += 1;
                return Token(SyntaxKind.PlusToken, start);
            case ['/', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.SlashEqualsToken, start);
            case ['/', ..]:
                _position += 1;
                return Token(SyntaxKind.SlashToken, start);
            case ['*', '*', '=', ..]:
                _position += 3;
                return Token(SyntaxKind.StarStarEqualsToken, start);
            case ['*', '*', ..]:
                _position += 2;
                return Token(SyntaxKind.AsteriskAsteriskToken, start);
            case ['*', '=', ..]:
                _position += 2;
                return Token(SyntaxKind.StarEqualsToken, start);
            case ['*', ..]:
                _position += 1;
                return Token(SyntaxKind.AsteriskToken, start);
            case ['~', ..]:
                _position += 1;
                return Token(SyntaxKind.TildeToken, start);
            case ['"', ..]: return ScanString();
            case [var d1, ..] when char.IsAsciiDigit(d1): return ScanNumber();
            case ['.', var d2, ..] when char.IsAsciiDigit(d2): return ScanNumber();
            case ['_', ..]: return ScanIdentifier();
            case [var l, ..] when char.IsAsciiLetter(l): return ScanIdentifier();
            case []: return Token(SyntaxKind.EofToken, start);
            default:
                _position += 1;
                var sourceSpan = new SourceSpan(sourceText, start.._position);
                _diagnostics.ReportInvalidCharacter(sourceSpan, sourceText[start]);
                return (SyntaxKind.InvalidSyntaxToken, sourceSpan, null);
        }

        (SyntaxKind, SourceSpan, object?) Token(SyntaxKind kind, int tokenStart) =>
            (kind, new SourceSpan(sourceText, tokenStart.._position), null);
    }

    private (SyntaxKind, SourceSpan, object? Value) ScanString()
    {
        var done = false;
        var start = _position;
        var terminated = false;
        var builder = new StringBuilder();
        _position++;

        while (!done)
        {
            switch (sourceText[_position..])
            {
                case ['\r' or '\n' or '\0', ..]:
                    done = true;
                    break;
                case ['\\', '"', ..]:
                    builder.Append('"');
                    _position += 2;
                    continue;
                case ['"', ..]:
                    _position += 1;
                    done = terminated = true;
                    break;
                default:
                    builder.Append(sourceText[_position]);
                    _position += 1;
                    continue;
            }
        }

        var sourceSpan = new SourceSpan(sourceText, start.._position);
        if (!terminated) _diagnostics.ReportUnterminatedString(sourceSpan);

        return (SyntaxKind.StrLiteralToken, sourceSpan, builder.ToString());
    }

    private (SyntaxKind, SourceSpan, object? Value) ScanNumber()
    {
        var start = _position;
        var read = 0;
        var isFloat = false;
        var isInvalid = false;
        var numberBase = 10;

        if (MatchesText(start + read, "0b"))
        {
            read += 2;
            numberBase = 2;
            var digitsStart = read;
            while (sourceText[start + read] is '0' or '1') read++;
            isInvalid |= read == digitsStart;
        }
        else if (MatchesText(start + read, "0x") || MatchesText(start + read, "0X"))
        {
            read += 2;
            numberBase = 16;
            var digitsStart = read;
            while (IsHexDigit(sourceText[start + read])) read++;
            isInvalid |= read == digitsStart;
        }
        else
        {
            while (char.IsAsciiDigit(sourceText[start + read])) read++;

            if (sourceText[start + read] == '.')
            {
                isFloat = true;
                read++;
                while (char.IsAsciiDigit(sourceText[start + read])) read++;
            }

            if (sourceText[start + read] is 'e' or 'E')
            {
                isFloat = true;
                if (!char.IsAsciiDigit(sourceText[start + read - 1])) isInvalid = true;
                read++;
                if (sourceText[start + read] is '+' or '-') read++;
                var exponentDigitsStart = read;
                while (char.IsAsciiDigit(sourceText[start + read])) read++;
                isInvalid |= read == exponentDigitsStart;
            }
        }

        var suffix = ScanNumberSuffix(start + read, isFloat);
        read += suffix.Length;
        var numberSpan = sourceText[start..(start + read - suffix.Length)];

        SyntaxKind syntaxKind;
        object value;

        if (isFloat || suffix.SyntaxKind is SyntaxKind.F16LiteralToken or SyntaxKind.F32LiteralToken or SyntaxKind.F64LiteralToken)
        {
            syntaxKind = suffix.SyntaxKind ?? SyntaxKind.F64LiteralToken;
            if (!double.TryParse(numberSpan, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                parsed = 0;
                isInvalid = true;
            }

            value = syntaxKind switch
            {
                SyntaxKind.F16LiteralToken => (Half)parsed,
                SyntaxKind.F32LiteralToken => (float)parsed,
                _ => parsed
            };

            isInvalid |= numberBase != 10 || !double.IsFinite(parsed);
        }
        else
        {
            var parsed = ParseInteger(numberSpan, numberBase);
            isInvalid |= parsed is null;
            var integer = parsed ?? 0;

            switch (suffix.SyntaxKind)
            {
                case SyntaxKind.I8LiteralToken:
                    syntaxKind = SyntaxKind.I8LiteralToken;
                    isInvalid |= integer < sbyte.MinValue || integer > sbyte.MaxValue;
                    value = integer is >= sbyte.MinValue and <= sbyte.MaxValue ? (sbyte)integer : (sbyte)0;
                    break;
                case SyntaxKind.U8LiteralToken:
                    syntaxKind = SyntaxKind.U8LiteralToken;
                    isInvalid |= integer < byte.MinValue || integer > byte.MaxValue;
                    value = integer is >= byte.MinValue and <= byte.MaxValue ? (byte)integer : (byte)0;
                    break;
                case SyntaxKind.I16LiteralToken:
                    syntaxKind = SyntaxKind.I16LiteralToken;
                    isInvalid |= integer < short.MinValue || integer > short.MaxValue;
                    value = integer is >= short.MinValue and <= short.MaxValue ? (short)integer : (short)0;
                    break;
                case SyntaxKind.U16LiteralToken:
                    syntaxKind = SyntaxKind.U16LiteralToken;
                    isInvalid |= integer < ushort.MinValue || integer > ushort.MaxValue;
                    value = integer is >= ushort.MinValue and <= ushort.MaxValue ? (ushort)integer : (ushort)0;
                    break;
                case SyntaxKind.I32LiteralToken:
                    syntaxKind = SyntaxKind.I32LiteralToken;
                    isInvalid |= integer < int.MinValue || integer > int.MaxValue;
                    value = integer is >= int.MinValue and <= int.MaxValue ? (int)integer : 0;
                    break;
                case SyntaxKind.U32LiteralToken:
                    syntaxKind = SyntaxKind.U32LiteralToken;
                    isInvalid |= integer < uint.MinValue || integer > uint.MaxValue;
                    value = integer is >= uint.MinValue and <= uint.MaxValue ? (uint)integer : 0u;
                    break;
                case SyntaxKind.I64LiteralToken:
                    syntaxKind = SyntaxKind.I64LiteralToken;
                    isInvalid |= integer < long.MinValue || integer > long.MaxValue;
                    value = integer is >= long.MinValue and <= long.MaxValue ? (long)integer : 0L;
                    break;
                case SyntaxKind.U64LiteralToken:
                    syntaxKind = SyntaxKind.U64LiteralToken;
                    isInvalid |= integer < ulong.MinValue || integer > ulong.MaxValue;
                    value = integer is >= ulong.MinValue and <= ulong.MaxValue ? (ulong)integer : 0UL;
                    break;
                default:
                    if (integer is >= int.MinValue and <= int.MaxValue)
                    {
                        syntaxKind = SyntaxKind.I32LiteralToken;
                        value = (int)integer;
                    }
                    else
                    {
                        syntaxKind = SyntaxKind.I64LiteralToken;
                        isInvalid |= integer < long.MinValue || integer > long.MaxValue;
                        value = integer is >= long.MinValue and <= long.MaxValue ? (long)integer : 0L;
                    }

                    break;
            }
        }

        _position = start + read;
        var sourceSpan = new SourceSpan(sourceText, start.._position);
        if (isInvalid) _diagnostics.ReportInvalidSyntaxValue(sourceSpan, syntaxKind);
        return (syntaxKind, sourceSpan, value);
    }

    private (SyntaxKind, SourceSpan, object? Value) ScanIdentifier()
    {
        var start = _position;
        do { _position++; } while (IsIdentifierPart(sourceText[_position]));

        var sourceSpan = new SourceSpan(sourceText, start.._position);
        var syntaxKind = SyntaxFacts.GetKeywordKind(sourceSpan.TextSpan);
        object? value = syntaxKind switch
        {
            SyntaxKind.TrueKeyword => true,
            SyntaxKind.FalseKeyword => false,
            _ => null
        };

        return (syntaxKind, sourceSpan, value);
    }

    private SyntaxList<SyntaxTrivia> ScanTrivia()
    {
        var trivia = new List<SyntaxTrivia>();
        while (true)
        {
            if (sourceText[_position..] is ['/', '*', ..]) trivia.Add(ScanMultiLineComment());
            else if (sourceText[_position..] is ['/', '/', ..]) trivia.Add(ScanSingleLineComment());
            else if (sourceText[_position..] is ['\r', '\n', ..] or ['\r', ..] or ['\n', ..]) trivia.Add(ScanLineBreak());
            else if (char.IsWhiteSpace(sourceText[_position]) && sourceText[_position] is not '\0') trivia.Add(ScanWhiteSpace());
            else break;
        }

        return [.. trivia];
    }

    private SyntaxTrivia ScanMultiLineComment()
    {
        var start = _position;
        var terminated = false;
        _position += 2;

        while (true)
        {
            if (sourceText[_position] == '\0') break;
            if (sourceText[_position..] is ['*', '/', ..])
            {
                _position += 2;
                terminated = true;
                break;
            }

            _position++;
        }

        var sourceSpan = new SourceSpan(sourceText, start.._position);
        if (!terminated) _diagnostics.ReportUnterminatedComment(sourceSpan);
        return new SyntaxTrivia(SyntaxKind.MultiLineCommentTrivia, sourceSpan);
    }

    private SyntaxTrivia ScanSingleLineComment()
    {
        var start = _position;
        _position += 2;
        while (sourceText[_position] is not ('\r' or '\n' or '\0')) _position++;
        return new SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, new SourceSpan(sourceText, start.._position));
    }

    private SyntaxTrivia ScanLineBreak()
    {
        var start = _position;
        _position += sourceText[_position..] is ['\r', '\n', ..] ? 2 : 1;
        return new SyntaxTrivia(SyntaxKind.LineBreakTrivia, new SourceSpan(sourceText, start.._position));
    }

    private SyntaxTrivia ScanWhiteSpace()
    {
        var start = _position;
        while (char.IsWhiteSpace(sourceText[_position]) && sourceText[_position] is not ('\r' or '\n' or '\0')) _position++;
        return new SyntaxTrivia(SyntaxKind.WhiteSpaceTrivia, new SourceSpan(sourceText, start.._position));
    }

    private (int Length, SyntaxKind? SyntaxKind) ScanNumberSuffix(int position, bool isFloat)
    {
        if (MatchesText(position, "f16")) return (3, SyntaxKind.F16LiteralToken);
        if (MatchesText(position, "f32")) return (3, SyntaxKind.F32LiteralToken);
        if (MatchesText(position, "f64")) return (3, SyntaxKind.F64LiteralToken);
        if (isFloat) return (0, null);
        if (MatchesText(position, "i8")) return (2, SyntaxKind.I8LiteralToken);
        if (MatchesText(position, "u8")) return (2, SyntaxKind.U8LiteralToken);
        if (MatchesText(position, "i16")) return (3, SyntaxKind.I16LiteralToken);
        if (MatchesText(position, "u16")) return (3, SyntaxKind.U16LiteralToken);
        if (MatchesText(position, "i32")) return (3, SyntaxKind.I32LiteralToken);
        if (MatchesText(position, "u32")) return (3, SyntaxKind.U32LiteralToken);
        if (MatchesText(position, "i64")) return (3, SyntaxKind.I64LiteralToken);
        if (MatchesText(position, "u64")) return (3, SyntaxKind.U64LiteralToken);
        return (0, null);
    }

    private bool MatchesText(int position, string text)
    {
        for (var i = 0; i < text.Length; i++)
            if (sourceText[position + i] != text[i])
                return false;
        return true;
    }

    private static bool IsIdentifierPart(char ch) => ch == '_' || char.IsAsciiLetterOrDigit(ch);
    private static bool IsHexDigit(char ch) => char.IsAsciiDigit(ch) || ch is >= 'a' and <= 'f' or >= 'A' and <= 'F';

    private static decimal? ParseInteger(ReadOnlySpan<char> text, int numberBase)
    {
        try
        {
            var digits = numberBase is 2 or 16 ? text[2..] : text;
            decimal value = 0;
            foreach (var ch in digits)
            {
                var digit = ch switch
                {
                    >= '0' and <= '9' => ch - '0',
                    >= 'a' and <= 'f' => ch - 'a' + 10,
                    >= 'A' and <= 'F' => ch - 'A' + 10,
                    _ => -1
                };
                if (digit < 0 || digit >= numberBase) return null;
                value = checked(value * numberBase + digit);
            }

            return value;
        }
        catch (OverflowException)
        {
            return null;
        }
    }
}
