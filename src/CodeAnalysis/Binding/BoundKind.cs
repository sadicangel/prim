namespace CodeAnalysis.Binding;

internal enum BoundKind
{
    Unbound,

    CompilationUnit,

    NopExpression,
    NeverExpression,

    LiteralExpression,

    AssignmentExpression,

    LabelDeclaration,
    StructDeclaration,
    VariableDeclaration,
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

    BreakExpression,
    ContinueExpression,
    ReturnExpression,
    GotoExpression,
    ConditionalGotoExpression,

    PropertySymbol,
    MethodSymbol,
    VariableSymbol,
    LabelSymbol,

    ArrayTypeSymbol,
    LambdaTypeSymbol,
    OptionTypeSymbol,
    StructTypeSymbol,
    UnionTypeSymbol,
}
