using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests.Interpretation;

public partial class InterpreterTests
{
    private readonly ScopeValue _scope = ModuleValue.CreateGlobalModule(ModuleSymbol.CreateGlobalModule());
}
