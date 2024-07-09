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

    UnaryExpression,
    BinaryExpression,

    IfElseExpression,
    WhileExpression,

    FunctionSymbol,
    StructSymbol,
    PropertySymbol,
    VariableSymbol,
}
