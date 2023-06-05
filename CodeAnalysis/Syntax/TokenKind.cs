﻿namespace CodeAnalysis.Syntax;

public enum TokenKind
{
    Invalid,
    WhiteSpace,
    Semicolon,
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
    EqualsEquals,
    BangEquals,
    Less,
    LessEquals,
    Greater,
    GreaterEquals,
    AmpersandAmpersand,
    PipePipe,
    True,
    False,
    Const,
    Var,
    If,
    Else,
    Identifier,
}
