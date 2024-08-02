using System.Collections.Concurrent;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Lowering;

partial class Lowerer
{
    private sealed record class Context(IBoundScope BoundScope)
    {
        private readonly ConcurrentDictionary<string, int> _labelIds = [];

        public LabelSymbol CreateLabel(string prefix, ModuleSymbol containingModule)
        {
            var labelId = _labelIds.AddOrUpdate(prefix, 1, (_, id) => id + 1);
            return new($"{prefix}<{labelId}>", BoundScope.Never, containingModule);
        }
    }
}
