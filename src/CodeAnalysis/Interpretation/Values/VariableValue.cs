using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class VariableValue(PrimType Type, object Value) : PrimValue(Type)
{
    public override object Value { get; } = Value;

    public static VariableValue Unit { get; } = new VariableValue(PredefinedTypes.Unit, new Unit());
    public static VariableValue True { get; } = new VariableValue(PredefinedTypes.Bool, true);
    public static VariableValue False { get; } = new VariableValue(PredefinedTypes.Bool, false);
}

file sealed class Unit
{
    public override string ToString() => SyntaxFacts.GetText(SyntaxKind.NullKeyword)
        ?? throw new UnreachableException("Missing ToString method");
}

