﻿using CodeAnalysis.Binding;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
internal static partial class Interpreter
{
    public static PrimValue Evaluate(BoundTree boundTree, EvaluatedScope evaluatedScope)
    {
        var context = new InterpreterContext(evaluatedScope);
        var value = PrimValue.Unit as PrimValue;

        foreach (var node in boundTree.CompilationUnit.BoundNodes)
            value = EvaluateNode(node, context);

        return value;
    }
}
