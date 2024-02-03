using CodeAnalysis.Text;

namespace CodeAnalysis.Binding;

internal record class BoundProgram(IReadOnlyList<BoundNode> Nodes, DiagnosticBag Diagnostics);
