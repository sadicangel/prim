using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Operators;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Text;
using CodeAnalysis.Types;
using System.Diagnostics;
using System.Reflection;
using Expr = System.Linq.Expressions.Expression;

namespace CodeAnalysis.Evaluation;

internal static class Evaluator
{
    internal static EvaluationResult Evaluate(Environment environment, BoundProgram program)
    {
        var context = new EvaluateContext(environment, []);

        object value = Unit.Value;
        PrimType type = PredefinedTypes.Unit;
        foreach (var node in program.Nodes)
        {
            value = EvaluateBoundNode(context, node);
            type = (node as BoundExpression)?.Type ?? PredefinedTypes.Unit;
        }

        return new EvaluationResult(value, type, context.Diagnostics);
    }

    private static object EvaluateBoundNode(EvaluateContext context, BoundNode boundNode)
    {
        return boundNode.NodeKind switch
        {
            //BoundNodeKind.GlobalExpression => EvaluateGlobalExpression(context, (BoundGlobalExpression)boundNode),
            BoundNodeKind.BlockExpression => EvaluateBlockExpression(context, (BoundBlockExpression)boundNode),
            BoundNodeKind.LiteralExpression => EvaluateLiteralExpression(context, (BoundLiteralExpression)boundNode),
            BoundNodeKind.UnaryExpression => EvaluateUnaryExpression(context, (BoundUnaryExpression)boundNode),
            BoundNodeKind.ArgumentList => EvaluateArgumentListExpression(context, (BoundArgumentListExpression)boundNode),
            BoundNodeKind.BinaryExpression => EvaluateBinaryExpression(context, (BoundBinaryExpression)boundNode),
            BoundNodeKind.DeclarationExpression => EvaluateDeclarationExpression(context, (BoundDeclarationExpression)boundNode),
            BoundNodeKind.AssignmentExpression => EvaluateAssignmentExpression(context, (BoundAssignmentExpression)boundNode),
            BoundNodeKind.NameExpression => EvaluateNameExpression(context, (BoundNameExpression)boundNode),
            //BoundNodeKind.ForExpression => EvaluateForExpression(context, (BoundForExpression)boundNode),
            //BoundNodeKind.IfElseExpression => EvaluateIfElseExpression(context, (BoundIfElseExpression)boundNode),
            //BoundNodeKind.WhileExpression => EvaluateWhileExpression(context, (BoundWhileExpression)boundNode),
            //BoundNodeKind.BreakExpression => EvaluateBreakExpression(context, (BoundBreakExpression)boundNode),
            //BoundNodeKind.ContinueExpression => EvaluateContinueExpression(context, (BoundContinueExpression)boundNode),
            //BoundNodeKind.ReturnExpression => EvaluateReturnExpression(context, (BoundReturnExpression)boundNode),
            //BoundNodeKind.ConvertExpression => throw new NotImplementedException(),
            //BoundNodeKind.NeverExpression => throw new NotImplementedException(),
            //BoundNodeKind.Symbol => throw new NotImplementedException(),
            //BoundNodeKind.Operator => throw new NotImplementedException(),
            _ => throw new UnreachableException($"Unexpected {nameof(BoundNodeKind)} '{boundNode.NodeKind}'"),
        };
    }

    private static object EvaluateBlockExpression(EvaluateContext context, BoundBlockExpression boundNode)
    {
        object result = Unit.Value;
        foreach (var expression in boundNode.Expressions)
            result = EvaluateBoundNode(context, expression);
        return result;
    }

    private static object EvaluateLiteralExpression(EvaluateContext context, BoundLiteralExpression boundNode)
    {
        return boundNode.Value ?? throw new UnreachableException("Value should not be null");
    }

    private static object EvaluateUnaryExpression(EvaluateContext context, BoundUnaryExpression boundNode)
    {
        dynamic operand = EvaluateBoundNode(context, boundNode.Operand);

        return boundNode.Operator.OperatorKind switch
        {
            OperatorKind.UnaryPlus => +operand,
            OperatorKind.Negate => -operand,
            OperatorKind.Increment => ++operand,
            OperatorKind.Decrement => --operand,
            OperatorKind.OnesComplement => ~operand,
            OperatorKind.Not => !operand,
            _ => throw new UnreachableException($"Unexpected {nameof(OperatorKind)} '{boundNode.Operator.OperatorKind}'"),
        };
    }

    private static object[] EvaluateArgumentListExpression(EvaluateContext context, BoundArgumentListExpression boundNode)
    {
        var args = new object[boundNode.Arguments.Count];
        for (int i = 0; i < args.Length; ++i)
            args[i] = EvaluateBoundNode(context, boundNode.Arguments[i]);
        return args;
    }

    private static object EvaluateBinaryExpression(EvaluateContext context, BoundBinaryExpression boundNode)
    {
        dynamic left = EvaluateBoundNode(context, boundNode.Left);
        dynamic right = EvaluateBoundNode(context, boundNode.Right);

        return boundNode.Operator.OperatorKind switch
        {
            OperatorKind.Add => left + right,
            OperatorKind.Subtract => left - right,
            OperatorKind.Multiply => left * right,
            OperatorKind.Divide => left / right,
            OperatorKind.Modulo => left % right,
            OperatorKind.Exponent => Convert.ChangeType(Math.Pow((double)left, (double)right), left?.GetType() ?? typeof(object))!,
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
        };
    }

    private static Symbol EvaluateDeclarationExpression(EvaluateContext context, BoundDeclarationExpression boundNode)
    {
        switch (boundNode.DeclarationKind)
        {
            case DeclarationKind.Variable:
                context.Environment.Declare(boundNode.Symbol, EvaluateBoundNode(context, boundNode.Expression));
                return boundNode.Symbol;

            case DeclarationKind.Function:
                context.Environment.Declare(boundNode.Symbol, CreateFunc(context, (FunctionType)boundNode.Symbol.Type, boundNode.Expression));
                return boundNode.Symbol;

            default:
                throw new UnreachableException($"Unexpected {nameof(DeclarationKind)} '{boundNode.DeclarationKind}'");
        }
    }

    private static Symbol EvaluateAssignmentExpression(EvaluateContext context, BoundAssignmentExpression boundNode)
    {
        switch (boundNode.Symbol.Type)
        {
            case FunctionType function:
                context.Environment.Declare(boundNode.Symbol, CreateFunc(context, function, boundNode.Expression));
                return boundNode.Symbol;

            default:
                context.Environment.Declare(boundNode.Symbol, EvaluateBoundNode(context, boundNode.Expression));
                return boundNode.Symbol;
        }
    }


    private static Delegate CreateFunc(EvaluateContext context, FunctionType function, BoundExpression expression)
    {
        System.Linq.Expressions.Expression<Action<EvaluateContext, Symbol, object>> DeclareSymbol =
            (context, symbol, value) => context.Environment.Declare(symbol, value);

        var pushEnv = typeof(EvaluateContext).GetMethod(nameof(EvaluateContext.PushEnvironment))!;
        var popEnv = typeof(EvaluateContext).GetMethod(nameof(EvaluateContext.PopEnvironment))!;
        var environment = typeof(EvaluateContext).GetProperty(nameof(EvaluateContext.Environment))!;
        var declare = typeof(Environment).GetMethod(nameof(Environment.Declare))!;
        var symbol = typeof(Symbol).GetConstructor([typeof(string), typeof(PrimType), typeof(bool)])!;
        var interpret = typeof(Evaluator).GetMethod(nameof(EvaluateBoundNode), BindingFlags.NonPublic | BindingFlags.Static)!;

        var parameters = function.Parameters.Select(p => Expr.Parameter(typeof(object), p.Name)).ToArray();

        var result = Expr.Variable(typeof(object), "result");
        var ctx = Expr.Constant(context, typeof(EvaluateContext));
        var node = Expr.Constant(expression, typeof(BoundExpression));
        var env = Expr.Property(ctx, environment);

        var body = Expr.Block(
            [result],
            Expr.Call(ctx, pushEnv),
            Expr.Block(function.Parameters.Select((p, i) => Expr.Call(env, declare, Expr.New(symbol, Expr.Constant(p.Name), Expr.Constant(p.Type), Expr.Constant(false)), parameters[i]))),
            Expr.Assign(result, Expr.Call(interpret, ctx, node)),
            Expr.Call(ctx, popEnv),
            result
        );
        var lambda = Expr.Lambda(body, parameters);
        var func = lambda.Compile();
        return func;
    }

    private static object EvaluateNameExpression(EvaluateContext context, BoundNameExpression boundNode)
    {
        return context.Environment.Lookup(boundNode.Symbol);
    }
}

internal sealed record class EvaluateContext(Environment Environment, DiagnosticBag Diagnostics)
{
    public Environment Environment { get; set; } = Environment;
    public void PushEnvironment() => Environment = Environment.CreateChildScope();
    public void PopEnvironment() => Environment = Environment.Parent ?? throw new InvalidOperationException("Invalid environment stack");
}
