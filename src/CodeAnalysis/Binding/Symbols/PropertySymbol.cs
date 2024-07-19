﻿using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    Symbol ContainingSymbol,
    bool IsReadOnly,
    bool IsStatic)
    : Symbol(
        BoundKind.PropertySymbol,
        Syntax,
        Name,
        Type,
        IsReadOnly,
        IsStatic);
