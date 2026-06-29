namespace CodeAnalysis.Semantic;

internal enum NodeKind
{
    // Error / sentinel
    Unbound,

    // Declarations
    Declaration,

    // Terminated expression (value discarded).
    ExpressionStatement,

    // References
    ModuleReference,
    TypeReference,
    VariableReference,
    ElementReference,
    MemberReference,

    // Basic expressions
    LiteralExpression,
    LambdaExpression,
    BlockExpression,
    ArrayInitializerExpression,
    InvocationExpression,
    ModuleInitializerExpression,
    TypeInitializerExpression,
    ObjectInitializerExpression,
    PropertyInitializerExpression,

    // Operators
    AssignmentExpression,
    UnaryExpression,
    BinaryExpression,

    // Control-flow expressions
    IfElseExpression,
    WhileExpression,
    BreakExpression,
    ContinueExpression,
    ReturnExpression,
    GotoExpression,
    GotoIfExpression,

    // Lowering / special expressions
    NopExpression,
    NeverExpression,
}
