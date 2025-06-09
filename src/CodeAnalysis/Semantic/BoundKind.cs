namespace CodeAnalysis.Semantic;

internal enum BoundKind
{
    Unbound,

    CompilationUnit,

    NopExpression,
    NeverExpression,

    StackInstantiation,

    LiteralExpression,
    LambdaExpression,

    AssignmentExpression,

    ModuleDeclaration,
    LabelDeclaration,
    StructDeclaration,
    VariableDeclaration,
    PropertyDeclaration,
    MethodDeclaration,
    OperatorDeclaration,
    ConversionDeclaration,

    LocalReference,
    GlobalReference,
    PropertyReference,
    MethodReference,
    MethodGroup,
    IndexReference,

    BlockExpression,
    ArrayInitExpression,
    StructInitExpression,
    PropertyInitExpression,
    InvocationExpression,

    ConversionExpression,

    UnaryExpression,
    BinaryExpression,

    IfExpression,
    WhileExpression,

    BreakExpression,
    ContinueExpression,
    ReturnExpression,
    GotoExpression,
    ConditionalGotoExpression,
}
