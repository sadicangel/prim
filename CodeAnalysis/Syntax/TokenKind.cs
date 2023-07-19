namespace CodeAnalysis.Syntax;

public enum TokenKind
{
    // Control
    Invalid,
    EOF,

    // Punctuation
    Arrow,
    OpenBrace,
    OpenParenthesis,
    CloseBrace,
    CloseParenthesis,
    Colon,
    Comma,
    Semicolon,

    // Trivia
    WhiteSpace,
    SingleLineComment,
    MultiLineComment,

    // Operators
    Ampersand,
    AmpersandAmpersand,
    As,
    Bang,
    BangEqual,
    Equal,
    EqualEqual,
    Greater,
    GreaterEqual,
    Hat,
    Less,
    LessEqual,
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
    Let,
    Var,
    If,
    Else,
    While,
    For,
    In,
    Continue,
    Break,
    Return,

    // Variable
    I32,
    F32,
    String,
    Identifier,
}
