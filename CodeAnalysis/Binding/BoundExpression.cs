namespace CodeAnalysis.Binding;

internal abstract record class BoundExpression(BoundNodeKind Kind, Type Type) : BoundNode(Kind);
