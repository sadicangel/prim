namespace CodeAnalysis.Syntax;

public enum TokenKind
{
    // Control
    Invalid,
    EOF,

    // Punctuation
    OpenBrace,
    OpenParenthesis,
    CloseBrace,
    CloseParenthesis,
    Semicolon,

    // Operatos
    Ampersand,
    AmpersandAmpersand,
    Bang,
    BangEquals,
    Equals,
    EqualsEquals,
    Greater,
    GreaterEquals,
    Hat,
    Less,
    LessEquals,
    Minus,
    Percent,
    Pipe,
    PipePipe,
    Plus,
    Range,
    Slash,
    Star,
    Tilde,

    // Keywords
    True,
    False,
    Const,
    Var,
    If,
    Else,
    While,
    For,
    In,

    // Variable
    WhiteSpace,
    I32,
    Identifier,
}
