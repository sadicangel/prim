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

    IfExpression,
    WhileExpression,

    FunctionSymbol,
    PropertySymbol,
    MethodSymbol,
    VariableSymbol,

    ArrayTypeSymbol,
    LambdaTypeSymbol,
    OptionTypeSymbol,
    StructTypeSymbol,
    UnionTypeSymbol,
    RuntimeTypeSymbol,
}
