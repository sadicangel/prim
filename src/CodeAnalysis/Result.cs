using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;

namespace CodeAnalysis;

public readonly record struct Result<T>(T Value, ImmutableArray<Diagnostic> Diagnostics);
