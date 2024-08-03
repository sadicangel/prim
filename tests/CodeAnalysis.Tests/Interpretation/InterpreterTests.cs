using CodeAnalysis.Binding;
using CodeAnalysis.Interpretation;

namespace CodeAnalysis.Tests.Interpretation;

public partial class InterpreterTests
{
    private readonly IEvaluatedScope _scope = IEvaluatedScope.CreateGlobalScope(IBoundScope.CreateGlobalScope());
}
