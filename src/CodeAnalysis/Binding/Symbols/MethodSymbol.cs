﻿using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class MethodSymbol(
    SyntaxKind MethodKind,
    SyntaxNode Syntax,
    string Name,
    LambdaTypeSymbol LambdaType,
    Symbol ContainingSymbol,
    bool IsStatic,
    bool IsReadOnly)
    : Symbol(
        BoundKind.MethodSymbol,
        Syntax,
        Name,
        LambdaType,
        IsStatic,
        IsReadOnly)
{
    public BoundList<VariableSymbol> Parameters { get => LambdaType.Parameters; }
    public TypeSymbol ReturnType { get => LambdaType.ReturnType; }

    public bool IsOperator { get => SyntaxFacts.IsOperator(MethodKind); }
    public bool IsUnaryOperator { get => IsOperator && Parameters.Count == 1; }
    public bool IsBinaryOperator { get => IsOperator && Parameters.Count == 2; }

    public bool IsConversion { get => IsImplicitConversion || IsExplicitConversion; }
    public bool IsImplicitConversion { get => MethodKind is SyntaxKind.ImplicitKeyword; }
    public bool IsExplicitConversion { get => MethodKind is SyntaxKind.ExplicitKeyword; }
}
