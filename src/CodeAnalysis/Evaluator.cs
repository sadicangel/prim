using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Operators;
using CodeAnalysis.Text;
using CodeAnalysis.Types;
using System.Collections;
using System.Diagnostics;

namespace CodeAnalysis;

internal static class Evaluator
{
    internal static Result Evaluate(Locals locals, BoundProgram program)
    {
        var context = new EvalContext(Unit.Value, locals, []);

        foreach (var node in program.Nodes)
            EvalBoundNode(ref context, node);

        return new Result(context.Value, context.Diagnostics);
    }

    private static object EvalBoundNode(ref EvalContext context, BoundNode boundNode)
    {
        return boundNode.NodeKind switch
        {
            //BoundNodeKind.GlobalExpression => EvalGlobalExpression(ref context, (BoundGlobalExpression)boundNode),
            //BoundNodeKind.BlockExpression => EvalBlockExpression(ref context, (BoundBlockExpression)boundNode),
            BoundNodeKind.LiteralExpression => EvalLiteralExpression(ref context, (BoundLiteralExpression)boundNode),
            //BoundNodeKind.UnaryExpression => EvalUnaryExpression(ref context, (BoundUnaryExpression)boundNode),
            BoundNodeKind.ArgumentList => EvalArgumentListExpression(ref context, (BoundArgumentListExpression)boundNode),
            BoundNodeKind.BinaryExpression => EvalBinaryExpression(ref context, (BoundBinaryExpression)boundNode),
            //BoundNodeKind.DeclarationExpression => EvalDeclarationExpression(ref context, (BoundDeclarationExpression)boundNode),
            //BoundNodeKind.AssignmentExpression => EvalAssignmentExpression(ref context, (BoundAssignmentExpression)boundNode),
            BoundNodeKind.NameExpression => EvalNameExpression(ref context, (BoundNameExpression)boundNode),
            //BoundNodeKind.ForExpression => EvalForExpression(ref context, (BoundForExpression)boundNode),
            //BoundNodeKind.IfElseExpression => EvalIfElseExpression(ref context, (BoundIfElseExpression)boundNode),
            //BoundNodeKind.WhileExpression => EvalWhileExpression(ref context, (BoundWhileExpression)boundNode),
            //BoundNodeKind.BreakExpression => EvalBreakExpression(ref context, (BoundBreakExpression)boundNode),
            //BoundNodeKind.ContinueExpression => EvalContinueExpression(ref context, (BoundContinueExpression)boundNode),
            //BoundNodeKind.ReturnExpression => EvalReturnExpression(ref context, (BoundReturnExpression)boundNode),
            //BoundNodeKind.ConvertExpression => throw new NotImplementedException(),
            //BoundNodeKind.NeverExpression => throw new NotImplementedException(),
            BoundNodeKind.Symbol => throw new NotImplementedException(),
            BoundNodeKind.Operator => throw new NotImplementedException(),
            _ => throw new UnreachableException($"Unexpected {nameof(BoundNodeKind)} '{boundNode.NodeKind}'"),
        };
    }

    private static object EvalLiteralExpression(ref EvalContext context, BoundLiteralExpression boundNode)
    {
        return context.Value = boundNode.Value ?? Unit.Value;
    }

    private static object EvalArgumentListExpression(ref EvalContext context, BoundArgumentListExpression boundNode)
    {
        var args = new object?[boundNode.Arguments.Count];
        for (int i = 0; i < args.Length; ++i)
            args[i] = EvalBoundNode(ref context, boundNode.Arguments[i]);
        return context.Value = args;
    }

    private static object EvalBinaryExpression(ref EvalContext context, BoundBinaryExpression boundNode)
    {
        dynamic left = EvalBoundNode(ref context, boundNode.Left);
        dynamic right = EvalBoundNode(ref context, boundNode.Right);

        return context.Value = boundNode.Operator.OperatorKind switch
        {
            OperatorKind.Add => left + right,
            OperatorKind.Subtract => left - right,
            OperatorKind.Multiply => left * right,
            OperatorKind.Divide => left / right,
            OperatorKind.Modulo => left % right,
            OperatorKind.Exponent => Convert.ChangeType(Math.Pow((double)left, (double)right), left?.GetType() ?? typeof(object)),
            OperatorKind.And => left & right,
            OperatorKind.Or => left | right,
            OperatorKind.ExclusiveOr => left ^ right,
            OperatorKind.LeftShift => left << right,
            OperatorKind.RightShift => left >> right,
            OperatorKind.Equal => left == right,
            OperatorKind.NotEqual => left != right,
            OperatorKind.LessThan => left < right,
            OperatorKind.LessThanOrEqual => left <= right,
            OperatorKind.GreaterThan => left > right,
            OperatorKind.GreaterThanOrEqual => left >= right,
            OperatorKind.AndAlso => left && right,
            OperatorKind.OrElse => left || right,
            OperatorKind.NullCoalescence => left ?? right,
            OperatorKind.Call => left.DynamicInvoke(right),
            _ => throw new UnreachableException($"Unexpected {nameof(OperatorKind)} '{boundNode.Operator.OperatorKind}'"),
        } ?? Unit.Value;
    }

    private static object EvalNameExpression(ref EvalContext context, BoundNameExpression boundNode)
    {
        return context.EvalLocal(boundNode.Symbol);
    }
}

internal sealed record class Unit
{
    public static readonly Unit Value = new();
    private Unit() { }
    public override string ToString() => PredefinedSymbolNames.Unit;
}

internal record struct EvalContext(object Value, Locals Locals, DiagnosticBag Diagnostics)
{
    public object EvalLocal(Symbol symbol) => Value = Locals.Lookup(symbol) ?? Unit.Value;
}

internal sealed record class Locals : IEnumerable<object?>
{
    private Dictionary<Symbol, object?>? _locals;

    private Locals() { }

    public Locals? Parent { get; init; }

    public bool IsGlobal { get => Parent is null; }

    public Locals CreateChildScope() => new() { Parent = this };

    public void Declare(Symbol symbol, object? value)
    {
        if (!(_locals ??= []).TryAdd(symbol, value))
            throw new UnreachableException($"{symbol.Name} redeclared");
    }

    public object? Lookup(Symbol symbol)
    {
        if (_locals is not null && _locals.TryGetValue(symbol, out var value))
            return value;

        if (Parent is not null)
            return Parent.Lookup(symbol);

        throw new UnreachableException($"{nameof(Parent)} was null");
    }

    public IEnumerator<object?> GetEnumerator() => _locals?.Values.GetEnumerator() ?? Enumerable.Empty<object?>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static Locals CreateGlobalSymbols()
    {
        var scope = new Locals();

        foreach (var symbol in PredefinedSymbols.All)
            scope.Declare(symbol, symbol.Type switch
            {
                TypeType type => type,
                FunctionType func => symbol.Name switch
                {
                    PredefinedSymbolNames.ScanLn => new Func<object?>(Console.ReadLine),
                    PredefinedSymbolNames.PrintLn => new Func<object?, object?>(obj =>
                    {
                        Console.WriteLine(obj);
                        return null;
                    }),
                    _ => throw new UnreachableException($"Unexpected predefined function '{symbol.Name}'")
                },
                _ => throw new UnreachableException($"Unexpected predefined symbol '{symbol.Name}'")
            });

        return scope;
    }
}