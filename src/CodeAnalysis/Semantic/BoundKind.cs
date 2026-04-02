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

    PredefinedDeclaration,
    ModuleDeclaration,
    LabelDeclaration,
    StructDeclaration,
    VariableDeclaration,
    PropertyDeclaration,
    MethodDeclaration,
    OperatorDeclaration,
    ConversionDeclaration,

    VariableReference,
    PropertyReference,
    ElementReference,

    BlockExpression,
    ArrayInitExpression,
    StructInitExpression,
    PropertyInitExpression,
    IndexExpression,
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