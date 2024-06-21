﻿namespace CodeAnalysis.Binding;

internal enum BoundKind
{
    Unbound,

    CompilationUnit,

    NeverExpression,

    IdentifierNameExpression,

    I32LiteralExpression,
    U32LiteralExpression,
    I64LiteralExpression,
    U64LiteralExpression,
    F32LiteralExpression,
    F64LiteralExpression,
    StrLiteralExpression,
    TrueLiteralExpression,
    FalseLiteralExpression,
    NullLiteralExpression,

    AssignmentExpression,

    VariableDeclaration,
    FunctionDeclaration,
    StructDeclaration,
    PropertyDeclaration,
    MethodDeclaration,
    OperatorDeclaration,
    ConversionDeclaration,

    FunctionBodyExpression,

    EmptyExpression,
    StatementExpression,
    BlockExpression,
    ArrayExpression,
    StructExpression,
    PropertyExpression,

    InvocationExpression,

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

    FunctionSymbol,
    StructSymbol,
    PropertySymbol,
    ConversionSymbol,
    MethodSymbol,
    VariableSymbol,

    UnaryPlusOperator,
    UnaryMinusOperator,
    OnesComplementOperator,
    NotOperator,

    AddOperator,
    SubtractOperator,
    MultiplyOperator,
    DivideOperator,
    ModuloOperator,
    PowerOperator,
    LeftShiftOperator,
    RightShiftOperator,
    LogicalOrOperator,
    LogicalAndOperator,
    BitwiseOrOperator,
    BitwiseAndOperator,
    ExclusiveOrOperator,
    EqualsOperator,
    NotEqualsOperator,
    LessThanOperator,
    LessThanOrEqualOperator,
    GreaterThanOperator,
    GreaterThanOrEqualOperator,
    CoalesceOperator,
}
