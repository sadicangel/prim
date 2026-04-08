using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class OperatorSymbol(
    OperatorKind OperatorKind,
    SyntaxNode Syntax,
    LambdaTypeSymbol LambdaType,
    StructTypeSymbol ContainingType)
    : Symbol(
        SymbolKind.Operator,
        Syntax,
        OperatorKind.GetOperatorName(LambdaType.Parameters.AsSpan()),
        LambdaType,
        ContainingType,
        ContainingType.ContainingModule,
        Modifiers.ReadOnly | Modifiers.Static);
