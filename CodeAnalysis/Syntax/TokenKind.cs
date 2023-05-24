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
    AmpersandAmpersand,
    PipePipe,
    EqualsEquals,
    BangEquals,
}
