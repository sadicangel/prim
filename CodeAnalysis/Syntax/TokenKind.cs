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
    Colon,
    Semicolon,
    Comma,

    // Operators
    Ampersand,
    AmpersandAmpersand,
    As,
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
    F32,
    String,
    Identifier,
}
