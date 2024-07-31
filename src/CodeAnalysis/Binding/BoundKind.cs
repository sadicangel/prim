namespace CodeAnalysis.Binding;

internal enum BoundKind
{
    Unbound,

    CompilationUnit,

    NopExpression,
    NeverExpression,

    LiteralExpression,

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

    StackInstantiationExpression,
    HeapInstantiationExpression,

    UnaryExpression,
    BinaryExpression,

    IfExpression,
    WhileExpression,

    BreakExpression,
    ContinueExpression,
    ReturnExpression,
    GotoExpression,
    ConditionalGotoExpression,

    ModuleSymbol,
    PropertySymbol,
    MethodSymbol,
    ConversionSymbol,
    VariableSymbol,
    LabelSymbol,

    ArrayTypeSymbol,
    ErrorTypeSymbol,
    LambdaTypeSymbol,
    OptionTypeSymbol,
    PointerTypeSymbol,
    StructTypeSymbol,
    UnionTypeSymbol,
}
