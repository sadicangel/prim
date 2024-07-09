namespace CodeAnalysis.Binding;

internal enum BoundKind
{
    Unbound,

    CompilationUnit,

    NeverExpression,

    LiteralExpression,

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
    MethodBodyExpression,

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
