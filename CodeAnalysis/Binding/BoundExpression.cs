namespace CodeAnalysis.Binding;

public abstract record class BoundExpression(BoundNodeKind Kind, Type Type) : BoundNode(Kind);
