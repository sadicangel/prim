namespace CodeAnalysis.Syntax;

public enum TokenKind
{
    Invalid,
    WhiteSpace,
    OpenParenthesis,
    CloseParenthesis,
    OpenBrace,
    CloseBrace,
    EOF,
    Int64,
    Plus,
    Minus,
    Star,
    Slash,
    Bang,
    Equals,
    AmpersandAmpersand,
    PipePipe,
    EqualsEquals,
    BangEquals,
    True,
    False,
    Let,
    Var,
    Identifier,
}
