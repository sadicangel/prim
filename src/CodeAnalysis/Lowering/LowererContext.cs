using System.Collections.Concurrent;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Lowering;
internal sealed record class LowererContext()
{
    private readonly ConcurrentDictionary<string, int> _labelIds = [];

    public LabelSymbol CreateLabel(string prefix)
    {
        var labelId = _labelIds.AddOrUpdate(prefix, 1, (_, id) => id + 1);
        return new($"{prefix}<{labelId}>");
    }
}
