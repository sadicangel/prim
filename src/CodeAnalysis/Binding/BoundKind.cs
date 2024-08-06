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

    ModuleSymbol,
    PropertySymbol,
    MethodSymbol,
    ConversionSymbol,
    VariableSymbol,
    LabelSymbol,
    AnonymousScopeSymbol,

    ArrayTypeSymbol,
    ErrorTypeSymbol,
    LambdaTypeSymbol,
    OptionTypeSymbol,
    PointerTypeSymbol,
    StructTypeSymbol,
    UnionTypeSymbol,
}
