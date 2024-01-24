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
    StarStar,
    StarStarEqual,
    Range,
    Slash,
    SlashEqual,
    Star,
    StarEqual,
    Tilde,

    // Control flow
    If,
    Else,
    While,
    For,
    Result,
    Continue,
    Break,
    Return,

    // Variables
    Mutable,
    I32,
    F32,
    String,
    True,
    False,
    Identifier,
}
