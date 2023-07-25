namespace CodeAnalysis.Syntax;

public enum TokenKind
{
    // Control
    Invalid,
    EOF,

    // Trivia
    InvalidText,
    LineBreak,
    WhiteSpace,
    SingleLineComment,
    MultiLineComment,

    // Punctuation
    Arrow,
    OpenBrace,
    OpenParenthesis,
    CloseBrace,
    CloseParenthesis,
    Colon,
    Comma,
    Semicolon,

    // Operators
    Ampersand,
    AmpersandAmpersand,
    AmpersandEqual,
    As,
    Bang,
    BangEqual,
    Equal,
    EqualEqual,
    Greater,
    GreaterEqual,
    Hat,
    HatEqual,
    Less,
    LessEqual,
    Minus,
    MinusEqual,
    Percent,
    PercentEqual,
    Pipe,
    PipeEqual,
    PipePipe,
    Plus,
    PlusEqual,
    Range,
    Slash,
    SlashEqual,
    Star,
    StarEqual,
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
