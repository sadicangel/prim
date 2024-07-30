using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    private readonly IBoundScope _scope = new AnonymousScope(Predefined.GlobalModule);
}
