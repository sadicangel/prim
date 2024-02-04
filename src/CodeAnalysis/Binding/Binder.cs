using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Operators;
using CodeAnalysis.Operators;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Text;
using CodeAnalysis.Types;
using System.Diagnostics;

namespace CodeAnalysis.Binding;
internal static class Binder
{
    public static BoundProgram Bind(Scope globalScope, CompilationUnit compilation)
    {
        var boundNodes = new List<BoundNode>();
        var context = new BindContext(globalScope, []);

        foreach (var syntaxNode in compilation.Nodes)
            boundNodes.Add(BindExpression(ref context, syntaxNode));

        return new BoundProgram(compilation, boundNodes, context.Diagnostics);
    }

    private static BoundExpression BindExpression(ref BindContext context, SyntaxNode syntaxNode)
    {
        return syntaxNode.NodeKind switch
        {
            SyntaxNodeKind.GlobalExpression => BindGlobalExpression(ref context, (GlobalExpression)syntaxNode),
            SyntaxNodeKind.BlockExpression => BindBlockExpression(ref context, (BlockExpression)syntaxNode),
            SyntaxNodeKind.InlineExpression => BindInlineExpression(ref context, (InlineExpression)syntaxNode),
            SyntaxNodeKind.EmptyExpression => BindEmptyExpression(ref context, (EmptyExpression)syntaxNode),
            SyntaxNodeKind.LiteralExpression => BindLiteralExpression(ref context, (LiteralExpression)syntaxNode),
            SyntaxNodeKind.GroupExpression => BindGroupExpression(ref context, (GroupExpression)syntaxNode),
            SyntaxNodeKind.UnaryExpression => BindUnaryExpression(ref context, (UnaryExpression)syntaxNode),
            SyntaxNodeKind.ArgumentList => BindArgumentListExpression(ref context, (ArgumentListExpression)syntaxNode),
            SyntaxNodeKind.BinaryExpression => BindBinaryExpression(ref context, (BinaryExpression)syntaxNode),
            SyntaxNodeKind.DeclarationExpression => BindDeclarationExpression(ref context, (DeclarationExpression)syntaxNode),
            SyntaxNodeKind.AssignmentExpression => BindAssignmentExpression(ref context, (AssignmentExpression)syntaxNode),
            SyntaxNodeKind.NameExpression => BindNameExpression(ref context, (NameExpression)syntaxNode),
            SyntaxNodeKind.ForExpression => BindForExpression(ref context, (ForExpression)syntaxNode),
            SyntaxNodeKind.IfElseExpression => BindIfElseExpression(ref context, (IfElseExpression)syntaxNode),
            SyntaxNodeKind.WhileExpression => BindWhileExpression(ref context, (WhileExpression)syntaxNode),
            SyntaxNodeKind.BreakExpression => BindBreakExpression(ref context, (BreakExpression)syntaxNode),
            SyntaxNodeKind.ContinueExpression => BindContinueExpression(ref context, (ContinueExpression)syntaxNode),
            SyntaxNodeKind.ReturnExpression => BindReturnExpression(ref context, (ReturnExpression)syntaxNode),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxNodeKind)} '{syntaxNode.NodeKind}'"),
        };
    }

    private static BoundGlobalExpression BindGlobalExpression(ref BindContext context, GlobalExpression syntaxNode)
    {
        return new BoundGlobalExpression(syntaxNode, BindDeclarationExpression(ref context, syntaxNode.Declaration));
    }

    private static BoundBlockExpression BindBlockExpression(ref BindContext context, BlockExpression syntaxNode)
    {
        context.PushScope();
        try
        {
            var expressions = new List<BoundExpression>(syntaxNode.Expressions.Count);
            foreach (var expression in syntaxNode.Expressions)
                expressions.Add(BindExpression(ref context, expression));
            return new BoundBlockExpression(syntaxNode, expressions);
        }
        finally
        {
            context.PopScope();
        }
    }

    private static BoundExpression BindInlineExpression(ref BindContext context, InlineExpression syntaxNode)
    {
        return BindExpression(ref context, syntaxNode.Expression);
    }

    private static BoundNeverExpression BindEmptyExpression(ref BindContext context, EmptyExpression syntaxNode)
    {
        // TODO: Should this be unit type instead?
        return new BoundNeverExpression(syntaxNode);
    }

    private static BoundExpression BindLiteralExpression(ref BindContext context, LiteralExpression syntaxNode)
    {
        if (!context.Scope.TryLookup(syntaxNode.Type.Name, out var symbol) || symbol.Type is not TypeType type)
        {
            context.Diagnostics.ReportUndefinedType(syntaxNode.Location, syntaxNode.Type);
            return new BoundNeverExpression(syntaxNode);
        }

        if (!type.Type.IsAssignableFrom(syntaxNode.Type))
        {
            context.Diagnostics.ReportInvalidExpressionType(syntaxNode.Location, type.Type, syntaxNode.Type);
            return new BoundNeverExpression(syntaxNode);
        }

        return new BoundLiteralExpression(syntaxNode, syntaxNode.Type, syntaxNode.Value);
    }

    private static BoundExpression BindGroupExpression(ref BindContext context, GroupExpression syntaxNode)
    {
        return BindExpression(ref context, syntaxNode.Expression);
    }

    private static BoundExpression BindUnaryExpression(ref BindContext context, UnaryExpression syntaxNode)
    {
        var operand = BindExpression(ref context, syntaxNode.Operand);
        if (operand.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return operand;
        }

        var operators = operand.Type.Operators.FindAll(op => op.OperatorKind == syntaxNode.Operator.TokenKind.GetUnaryOperatorKind());

        if (operators is not [var @operator])
        {
            if (operators.Count == 0)
            {
                context.Diagnostics.ReportUndefinedUnaryOperator(syntaxNode.Operator, operand.Type);
                return new BoundNeverExpression(syntaxNode);
            }

            throw new UnreachableException($"Unexpected number of unary operators. Expected 0 or 1, got {operators.Count}");
        }

        return new BoundUnaryExpression(syntaxNode, new BoundOperator(syntaxNode.Operator, @operator), operand);
    }

    private static BoundExpression BindArgumentListExpression(ref BindContext context, ArgumentListExpression syntaxNode)
    {
        var boundArguments = new List<BoundExpression>(syntaxNode.Arguments.Count);
        foreach (var argument in syntaxNode.Arguments)
        {
            var boundArgument = BindExpression(ref context, argument);
            if (boundArgument.Type == PredefinedTypes.Never)
            {
                // Diagnostic already reported as type is BoundNeverExpression.
                return boundArgument;
            }
            boundArguments.Add(boundArgument);
        }

        return new BoundArgumentListExpression(syntaxNode, boundArguments);
    }

    private static BoundExpression BindBinaryExpression(ref BindContext context, BinaryExpression syntaxNode)
    {
        var operatorKind = syntaxNode.Operator.TokenKind.GetBinaryOperatorKind();

        var left = BindExpression(ref context, syntaxNode.Left);
        if (left.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return left;
        }

        var right = BindExpression(ref context, syntaxNode.Right);
        if (right.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return right;
        }

        if (right is BoundArgumentListExpression argumentList)
        {
            if (left.Type is not FunctionType functionType)
            {
                functionType = new FunctionType([.. argumentList.Arguments.Select((a, i) => new Parameter($"arg{i}", a.Type))], argumentList.Type);
                context.Diagnostics.ReportInvalidSymbolType(syntaxNode.Left.Location, functionType, left.Type);
                return new BoundNeverExpression(syntaxNode);
            }

            if (argumentList.Arguments.Count != functionType.Parameters.Count)
            {
                context.Diagnostics.ReportInvalidNumberOfArguments(syntaxNode.Location, functionType, argumentList.Arguments.Count);
                return new BoundNeverExpression(syntaxNode);
            }

            for (int i = 0; i < functionType.Parameters.Count; ++i)
            {
                var parameter = functionType.Parameters[i];
                var argument = argumentList.Arguments[i];

                if (!parameter.Type.IsAssignableFrom(argument.Type))
                {
                    context.Diagnostics.ReportInvalidArgumentType(argument.SyntaxNode.Location, parameter, argument.Type);
                    return new BoundNeverExpression(syntaxNode);
                }
            }
        }

        var operators = left.Type.Operators
            .Concat(left.Type != right.Type ? right.Type.Operators : Enumerable.Empty<Operator>())
            .Where(op => op.OperatorKind == operatorKind)
            .Cast<BinaryOperator>()
            .Where(op => op.LeftType.IsAssignableFrom(left.Type) && op.RightType.IsAssignableFrom(right.Type))
            .ToList();

        if (operators is not [var @operator])
        {
            if (operators.Count == 0)
            {
                context.Diagnostics.ReportUndefinedBinaryOperator(syntaxNode.Operator, left.Type, right.Type);
                return new BoundNeverExpression(syntaxNode);
            }

            context.Diagnostics.ReportAmbiguousBinaryOperator(syntaxNode.Operator, left.Type, right.Type);
            return new BoundNeverExpression(syntaxNode);
        }

        return new BoundBinaryExpression(syntaxNode, left, new BoundOperator(syntaxNode.Operator, @operator), right);
    }

    private static BoundExpression BindDeclarationExpression(ref BindContext context, DeclarationExpression syntaxNode)
    {
        var symbol = new Symbol(syntaxNode.Identifier.Text.ToString(), syntaxNode.TypeNode.Type);
        if (!context.Scope.TryDeclare(symbol))
        {
            context.Diagnostics.ReportSymbolRedeclaration(syntaxNode.Identifier.Location, context.Scope.Lookup(symbol.Name)!);
            return new BoundNeverExpression(syntaxNode);
        }

        BoundExpression expression;
        if (symbol.Type is FunctionType functionType)
        {
            context.PushScope();
            foreach (var parameter in functionType.Parameters)
            {
                var paramSymbol = new Symbol(parameter.Name, parameter.Type);
                if (!context.Scope.TryDeclare(paramSymbol))
                {
                    context.Diagnostics.ReportSymbolRedeclaration(syntaxNode.Identifier.Location, context.Scope.Lookup(symbol.Name)!);
                    return new BoundNeverExpression(syntaxNode);
                }
            }

            expression = BindExpression(ref context, syntaxNode.Expression);
            if (!functionType.ReturnType.IsAssignableFrom(expression.Type))
            {
                context.Diagnostics.ReportInvalidExpressionType(syntaxNode.Location, symbol.Type, expression.Type);
                return new BoundNeverExpression(syntaxNode);
            }

            context.PopScope();

            functionType.Operators.Add(new BinaryOperator(
                OperatorKind.Call,
                functionType,
                new TypeList([.. functionType.Parameters.Select(p => p.Type)]),
                functionType.ReturnType));
        }
        else
        {
            expression = BindExpression(ref context, syntaxNode.Expression);
            if (!symbol.Type.IsAssignableFrom(expression.Type))
            {
                context.Diagnostics.ReportInvalidExpressionType(syntaxNode.Location, symbol.Type, expression.Type);
                return new BoundNeverExpression(syntaxNode);
            }
        }

        return new BoundDeclarationExpression(syntaxNode, symbol, expression);
    }

    private static BoundExpression BindAssignmentExpression(ref BindContext context, AssignmentExpression syntaxNode)
    {
        if (!context.Scope.TryLookup(syntaxNode.Identifier.Text.ToString(), out var symbol))
        {
            context.Diagnostics.ReportUndefinedSymbol(syntaxNode.Identifier.Location, new Symbol(syntaxNode.Identifier.Text.ToString(), PredefinedTypes.Never));
            return new BoundNeverExpression(syntaxNode);
        }
        if (!symbol.IsMutable)
        {
            context.Diagnostics.ReportSymbolReassignment(syntaxNode.Identifier.Location, symbol);
            return new BoundNeverExpression(syntaxNode);
        }

        var expression = !syntaxNode.IsCompound
            ? BindExpression(ref context, syntaxNode)
            : BindBinaryExpression(ref context, FromCompoundAssignment(syntaxNode));

        if (expression.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return expression;
        }
        if (!symbol.Type.IsAssignableFrom(expression.Type))
        {
            context.Diagnostics.ReportInvalidExpressionType(syntaxNode.Location, symbol.Type, expression.Type);
            return new BoundNeverExpression(syntaxNode);
        }

        return new BoundAssignmentExpression(syntaxNode, expression);

        static BinaryExpression FromCompoundAssignment(AssignmentExpression syntaxNode)
        {
            return new BinaryExpression(
                syntaxNode.SyntaxTree,
                new NameExpression(syntaxNode.SyntaxTree, syntaxNode.Identifier),
                new Token(
                    syntaxNode.Equal.SyntaxTree,
                    syntaxNode.Equal.TokenKind.GetBinaryOperatorOfAssignmentExpression(),
                    (syntaxNode.Equal.Range.Start.Value + 1)..syntaxNode.Equal.Range.End,
                    syntaxNode.Equal.Trivia,
                    syntaxNode.Equal.Value),
                syntaxNode.Expression);
        }
    }

    private static BoundExpression BindNameExpression(ref BindContext context, NameExpression syntaxNode)
    {
        if (syntaxNode.Identifier.IsArtificial)
        {
            // Diagnostic already reported as identifier was not parsed.
            return new BoundNeverExpression(syntaxNode);
        }

        if (!context.Scope.TryLookup(syntaxNode.Identifier.Text.ToString(), out var symbol))
        {
            context.Diagnostics.ReportUndefinedSymbol(syntaxNode.Location, new Symbol(syntaxNode.Identifier.Text.ToString(), PredefinedTypes.Never));
            return new BoundNeverExpression(syntaxNode);
        }

        return new BoundNameExpression(syntaxNode, symbol);
    }

    private static BoundExpression BindForExpression(ref BindContext context, ForExpression syntaxNode)
    {
        throw new NotImplementedException();
    }

    private static BoundExpression BindIfElseExpression(ref BindContext context, IfElseExpression syntaxNode)
    {
        var condition = BindExpression(ref context, syntaxNode.Condition);
        if (condition.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return condition;
        }
        if (!condition.Type.IsConvertibleTo(PredefinedTypes.Bool, out var conversion))
        {
            context.Diagnostics.ReportInvalidTypeConversion(syntaxNode.Condition.Location, condition.Type, PredefinedTypes.Bool);
            return new BoundNeverExpression(syntaxNode);
        }
        if ("identity" != (string?)conversion)
        {
            throw new NotImplementedException("Conversion");
        }

        var then = BindExpression(ref context, syntaxNode.Then);
        if (then.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return then;
        }

        var @else = BindExpression(ref context, syntaxNode.Else);
        if (@else.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return @else;
        }

        return new BoundIfElseExpression(syntaxNode, condition, then, @else);
    }

    private static BoundExpression BindWhileExpression(ref BindContext context, WhileExpression syntaxNode)
    {
        throw new NotImplementedException();
    }

    private static BoundBreakExpression BindBreakExpression(ref BindContext context, BreakExpression syntaxNode)
    {
        return new BoundBreakExpression(syntaxNode, BindExpression(ref context, syntaxNode.Result));
    }

    private static BoundContinueExpression BindContinueExpression(ref BindContext context, ContinueExpression syntaxNode)
    {
        return new BoundContinueExpression(syntaxNode, BindExpression(ref context, syntaxNode.Result));
    }

    private static BoundReturnExpression BindReturnExpression(ref BindContext context, ReturnExpression syntaxNode)
    {
        return new BoundReturnExpression(syntaxNode, BindExpression(ref context, syntaxNode.Result));
    }
}

internal delegate TResult BindContextFunc<TState, TResult>(ref BindContext context, TState state);

internal record struct BindContext(Scope Scope, DiagnosticBag Diagnostics)
{
    private readonly Stack<Scope> _stack = new([Scope]);

    public readonly Scope Scope { get => _stack.Peek(); init => _stack = new([value]); }

    public readonly void PushScope() => _stack.Push(Scope.CreateChildScope());

    public readonly void PopScope() => _stack.Pop();
}