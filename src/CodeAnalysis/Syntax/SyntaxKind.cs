﻿namespace CodeAnalysis.Syntax;
public enum SyntaxKind
{
    InvalidSyntax,

    // SyntaxTokens

    EofToken,
    IdentifierToken,

    I8LiteralToken,
    U8LiteralToken,
    I16LiteralToken,
    U16LiteralToken,
    I32LiteralToken,
    U32LiteralToken,
    I64LiteralToken,
    U64LiteralToken,
    F16LiteralToken,
    F32LiteralToken,
    F64LiteralToken,
    StrLiteralToken,

    AmpersandToken,
    AmpersandAmpersandToken,
    AmpersandEqualsToken,
    BangToken,
    BangEqualsToken,
    BraceOpenToken,
    BraceCloseToken,
    BracketOpenToken,
    BracketCloseToken,
    BracketOpenBracketCloseToken,
    ColonToken,
    ColonColonToken,
    CommaToken,
    DotToken,
    DotDotToken,
    EqualsToken,
    EqualsEqualsToken,
    MinusGreaterThanToken,
    GreaterThanToken,
    GreaterThanEqualsToken,
    GreaterThanGreaterThanToken,
    GreaterThanGreaterThanEqualsToken,
    HatToken,
    HatEqualsToken,
    HookToken,
    HookHookToken,
    HookHookEqualsToken,
    LessThanToken,
    LessThanEqualsToken,
    LessThanLessThanToken,
    LessThanLessThanEqualsToken,
    MinusToken,
    MinusEqualsToken,
    ParenthesisOpenToken,
    ParenthesisCloseToken,
    ParenthesisOpenParenthesisCloseToken,
    PercentToken,
    PercentEqualsToken,
    PipeToken,
    PipeEqualsToken,
    PipePipeToken,
    PlusToken,
    PlusEqualsToken,
    SemicolonToken,
    SlashToken,
    SlashEqualsToken,
    StarToken,
    StarEqualsToken,
    StarStarToken,
    StarStarEqualsToken,
    TildeToken,

    AsKeyword,
    IfKeyword,
    ImplicitKeyword,
    ElseKeyword,
    ExplicitKeyword,
    WhileKeyword,
    ForKeyword,
    ContinueKeyword,
    BreakKeyword,
    ReturnKeyword,

    AnyKeyword,
    ErrKeyword,
    UnknownKeyword,
    NeverKeyword,
    UnitKeyword,
    TypeKeyword,
    StrKeyword,
    BoolKeyword,
    I8Keyword,
    I16Keyword,
    I32Keyword,
    I64Keyword,
    IszKeyword,
    U8Keyword,
    U16Keyword,
    U32Keyword,
    U64Keyword,
    UszKeyword,
    F16Keyword,
    F32Keyword,
    F64Keyword,

    ModuleKeyword,
    StructKeyword,

    TrueKeyword,
    FalseKeyword,
    NullKeyword,

    ThisKeyword,

    LineBreakTrivia,
    WhiteSpaceTrivia,
    SingleLineCommentTrivia,
    MultiLineCommentTrivia,
    InvalidTextTrivia,

    // Other nodes

    ArrayType,
    ErrorType,
    LambdaType,
    NamedType,
    OptionType,
    PointerType,
    PredefinedType,
    UnionType,

    Parameter,
    Argument,

    CompilationUnit,

    // Expressions

    SimpleName,
    QualifiedName,

    I8LiteralExpression,
    U8LiteralExpression,
    I16LiteralExpression,
    U16LiteralExpression,
    I32LiteralExpression,
    U32LiteralExpression,
    I64LiteralExpression,
    U64LiteralExpression,
    F16LiteralExpression,
    F32LiteralExpression,
    F64LiteralExpression,
    StrLiteralExpression,
    TrueLiteralExpression,
    FalseLiteralExpression,
    NullLiteralExpression,

    GroupExpression,

    ModuleDeclaration,
    StructDeclaration,
    VariableDeclaration,
    PropertyDeclaration,
    MethodDeclaration,
    OperatorDeclaration,
    ConversionDeclaration,
    LocalDeclaration,

    EmptyExpression,
    StatementExpression,
    BlockExpression,
    ArrayExpression,
    StructInitExpression,
    PropertyInitExpression,

    IndexExpression,
    InvocationExpression,
    MemberAccessExpression,
    ConversionExpression,

    UnaryPlusExpression,
    UnaryMinusExpression,
    OnesComplementExpression,
    NotExpression,

    AddExpression,
    SubtractExpression,
    MultiplyExpression,
    DivideExpression,
    ModuloExpression,
    PowerExpression,
    LeftShiftExpression,
    RightShiftExpression,
    LogicalOrExpression,
    LogicalAndExpression,
    BitwiseOrExpression,
    BitwiseAndExpression,
    ExclusiveOrExpression,
    EqualsExpression,
    NotEqualsExpression,
    LessThanExpression,
    LessThanOrEqualExpression,
    GreaterThanExpression,
    GreaterThanOrEqualExpression,
    CoalesceExpression,

    AssignmentExpression,

    InitValueExpression,

    IfExpression,
    ElseClauseExpression,
    WhileExpression,

    ContinueExpression,
    BreakExpression,
    ReturnExpression,
}
