﻿using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public sealed record class EvaluationResult(object? Value, IEnumerable<Diagnostic> Diagnostics);

public sealed class Compilation
{
    public Compilation(SyntaxTree syntaxTree)
    {
        SyntaxTree = syntaxTree;
    }

    public SyntaxTree SyntaxTree { get; }

    public EvaluationResult Evaluate()
    {
        var binder = new Binder();
        var boundExpression = binder.BindExpression(SyntaxTree.Root);

        var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();
        if (diagnostics.Any())
        {
            return new EvaluationResult(null, diagnostics);
        }
        var evaluator = new Evaluator(boundExpression);
        var value = evaluator.Evaluate();

        return new EvaluationResult(value, Enumerable.Empty<Diagnostic>());
    }
}