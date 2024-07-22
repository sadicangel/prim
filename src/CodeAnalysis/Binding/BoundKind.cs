namespace CodeAnalysis.Binding;

internal enum BoundKind
{
    Unbound,

    CompilationUnit,

    NeverExpression,

    LiteralExpression,

    AssignmentExpression,

    VariableDeclaration,
    StructDeclaration,
    PropertyDeclaration,
    MethodDeclaration,
    OperatorDeclaration,
    ConversionDeclaration,

    LocalReference,
    PropertyReference,
    MethodReference,
    MethodGroup,
    IndexReference,

    BlockExpression,
    ArrayInitExpression,
    StructInitExpression,
    PropertyInitExpression,
    InvocationExpression,

    UnaryExpression,
    BinaryExpression,

    IfExpression,
    WhileExpression,

    PropertySymbol,
    MethodSymbol,
    VariableSymbol,

    ArrayTypeSymbol,
    LambdaTypeSymbol,
    OptionTypeSymbol,
    StructTypeSymbol,
    UnionTypeSymbol,
}
