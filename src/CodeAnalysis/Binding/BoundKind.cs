namespace CodeAnalysis.Binding;

internal enum BoundKind
{
    Unbound,

    CompilationUnit,

    NeverExpression,

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

    LocalReference,
    MemberReference,
    IndexReference,

    FunctionBodyExpression,

    BlockExpression,
    ArrayInitExpression,
    StructInitExpression,
    PropertyInitExpression,
    InvocationExpression,

    ConversionExpression,

    UnaryExpression,
    BinaryExpression,

    IfElseExpression,
    WhileExpression,

    FunctionSymbol,
    StructSymbol,
    PropertySymbol,
    ConversionSymbol,
    MethodSymbol,
    VariableSymbol,

    IndexOperator,
    InvocationOperator,
    MemberAccessOperator,
    ConversionOperator,

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
