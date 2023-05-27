namespace CodeAnalysis.Syntax;

public enum TokenKind
{
    Invalid,
    WhiteSpace,
    Int64,
    Plus,
    Minus,
    Star,
    Slash,
    OpenParenthesis,
    CloseParenthesis,
    EOF,
    True,
    False,
    Identifier,
    Bang,
    Equals,
    AmpersandAmpersand,
    PipePipe,
    EqualsEquals,
    BangEquals,
}
