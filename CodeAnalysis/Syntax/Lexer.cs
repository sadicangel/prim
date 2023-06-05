using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

internal sealed class Lexer
{
    private readonly SourceText _text;
    private readonly DiagnosticBag _diagnostics = new();

    public Lexer(SourceText text)
    {
        _text = text;
    }

    public IEnumerable<Diagnostic> Diagnostics { get => _diagnostics; }

    private int _position;
    private int _start;
    private TokenKind _kind;
    private object? _value;

    private char Current { get => Peek(0); }

    private char Lookahead { get => Peek(1); }

    public bool IsEOF { get => _position >= _text.Length; }

    private char Peek(int offset)
    {
        var index = _position + offset;
        return index < _text.Length ? _text[index] : '\0';
    }

    public Token NextToken()
    {
        _start = _position;
        _kind = TokenKind.Invalid;
        _value = null;

        switch (Current)
        {
            case '\0':
                _kind = TokenKind.EOF;
                break;

            case ';':
                _kind = TokenKind.Semicolon;
                _position++;
                break;

            case '+':
                _kind = TokenKind.Plus;
                _position++;
                break;

            case '-':
                _kind = TokenKind.Minus;
                _position++;
                break;

            case '*':
                _kind = TokenKind.Star;
                _position++;
                break;

            case '/':
                _kind = TokenKind.Slash;
                _position++;
                break;

            case '(':
                _kind = TokenKind.OpenParenthesis;
                _position++;
                break;

            case ')':
                _kind = TokenKind.CloseParenthesis;
                _position++;
                break;

            case '{':
                _kind = TokenKind.OpenBrace;
                _position++;
                break;

            case '}':
                _kind = TokenKind.CloseBrace;
                _position++;
                break;

            case '!' when Lookahead is '=':
                _kind = TokenKind.BangEquals;
                _position += 2;
                break;

            case '!':
                _kind = TokenKind.Bang;
                _position++;
                break;

            case '=' when Lookahead is '=':
                _kind = TokenKind.EqualsEquals;
                _position += 2;
                break;

            case '=':
                _kind = TokenKind.Equals;
                _position++;
                break;

            case '&' when Lookahead is '&':
                _kind = TokenKind.AmpersandAmpersand;
                _position += 2;
                break;

            case '|' when Lookahead is '|':
                _kind = TokenKind.PipePipe;
                _position += 2;
                break;

            case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                ReadNumberToken();
                break;

            case 'A' or 'B' or 'C' or 'D' or 'E' or 'F' or 'G' or 'H' or 'I' or 'J' or 'K' or 'L' or 'M' or 'N' or 'O' or 'P' or 'Q' or 'R' or 'S' or 'T' or 'U' or 'V' or 'W' or 'X' or 'Y' or 'Z' or
                 'a' or 'b' or 'c' or 'd' or 'e' or 'f' or 'g' or 'h' or 'i' or 'j' or 'k' or 'l' or 'm' or 'n' or 'o' or 'p' or 'q' or 'r' or 's' or 't' or 'u' or 'v' or 'w' or 'x' or 'y' or 'z':
                ReadIdentifier();
                break;

            case ' ' or '\t' or '\n' or '\r':
                ReadWhiteSpace();
                break;

            case char when Char.IsWhiteSpace(Current):
                ReadWhiteSpace();
                break;

            default:
                _diagnostics.ReportInvalidCharacter(_position, Current);
                _kind = TokenKind.Invalid;
                _position++;
                break;
        }
        var text = SyntaxFacts.GetText(_kind) ?? _text[_start.._position];

        return new Token(_kind, _start, text.ToString(), _value);
    }

    private void ReadIdentifier()
    {
        do
        {
            _position++;
        }
        while (Char.IsLetter(Current));

        var text = _text[_start.._position];
        _kind = text.GetKeywordKind();
    }

    private void ReadWhiteSpace()
    {
        do
        {
            _position++;
        }
        while (Char.IsWhiteSpace(Current));

        _kind = TokenKind.WhiteSpace;
    }

    private void ReadNumberToken()
    {
        do
        {
            _position++;
        }
        while (Char.IsDigit(Current));

        var text = _text[_start.._position];
        if (!long.TryParse(text, out var value))
            _diagnostics.ReportInvalidNumber(TextSpan.FromBounds(_start, _position), text.ToString(), typeof(long));

        _kind = TokenKind.Int64;
        _value = value;
    }
}
