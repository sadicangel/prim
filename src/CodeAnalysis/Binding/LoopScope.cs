using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;
internal sealed record class LoopScope(LabelSymbol ContinueLabel, LabelSymbol BreakLabel);
