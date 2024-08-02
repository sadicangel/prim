using CodeAnalysis.Binding;
using CodeAnalysis.Interpretation;

namespace CodeAnalysis.Tests.Interpretation;

public partial class InterpreterTests
{
    private readonly EvaluatedScope _scope = new(IBoundScope.CreateGlobalScope());
}
