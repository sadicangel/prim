using CodeAnalysis.Diagnostics;
using CodeAnalysis.Evaluation.Values;

namespace CodeAnalysis.Evaluation;

internal readonly record struct EvaluatedResult(PrimValue Value, DiagnosticBag Diagnostics);
