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
            boundNodes.Add(BindExpression(context, syntaxNode));

        return new BoundProgram(compilation, boundNodes, context.Diagnostics);
    }

    private static BoundExpression BindExpression(BindContext context, SyntaxNode syntaxNode)
    {
        return syntaxNode.NodeKind switch
        {
            SyntaxNodeKind.GlobalExpression => BindGlobalExpression(context, (GlobalExpression)syntaxNode),
            SyntaxNodeKind.BlockExpression => BindBlockExpression(context, (BlockExpression)syntaxNode),
            SyntaxNodeKind.InlineExpression => BindInlineExpression(context, (InlineExpression)syntaxNode),
            SyntaxNodeKind.EmptyExpression => BindEmptyExpression(context, (EmptyExpression)syntaxNode),
            SyntaxNodeKind.LiteralExpression => BindLiteralExpression(context, (LiteralExpression)syntaxNode),
            SyntaxNodeKind.GroupExpression => BindGroupExpression(context, (GroupExpression)syntaxNode),
            SyntaxNodeKind.UnaryExpression => BindUnaryExpression(context, (UnaryExpression)syntaxNode),
            SyntaxNodeKind.ArgumentList => BindArgumentListExpression(context, (ArgumentListExpression)syntaxNode),
            SyntaxNodeKind.BinaryExpression => BindBinaryExpression(context, (BinaryExpression)syntaxNode),
            SyntaxNodeKind.DeclarationExpression => BindDeclarationExpression(context, (DeclarationExpression)syntaxNode),
            SyntaxNodeKind.AssignmentExpression => BindAssignmentExpression(context, (AssignmentExpression)syntaxNode),
            SyntaxNodeKind.NameExpression => BindNameExpression(context, (NameExpression)syntaxNode),
            SyntaxNodeKind.ForExpression => BindForExpression(context, (ForExpression)syntaxNode),
            SyntaxNodeKind.IfElseExpression => BindIfElseExpression(context, (IfElseExpression)syntaxNode),
            SyntaxNodeKind.WhileExpression => BindWhileExpression(context, (WhileExpression)syntaxNode),
            SyntaxNodeKind.BreakExpression => BindBreakExpression(context, (BreakExpression)syntaxNode),
            SyntaxNodeKind.ContinueExpression => BindContinueExpression(context, (ContinueExpression)syntaxNode),
            SyntaxNodeKind.ReturnExpression => BindReturnExpression(context, (ReturnExpression)syntaxNode),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxNodeKind)} '{syntaxNode.NodeKind}'"),
        };
    }

    private static BoundGlobalExpression BindGlobalExpression(BindContext context, GlobalExpression syntaxNode)
    {
        return new BoundGlobalExpression(syntaxNode, BindDeclarationExpression(context, syntaxNode.Declaration));
    }

    private static BoundBlockExpression BindBlockExpression(BindContext context, BlockExpression syntaxNode)
    {
        context.PushScope();
        try
        {
            var expressions = new List<BoundExpression>(syntaxNode.Expressions.Count);
            foreach (var expression in syntaxNode.Expressions)
                expressions.Add(BindExpression(context, expression));
            return new BoundBlockExpression(syntaxNode, expressions);
        }
        finally
        {
            context.PopScope();
        }
    }

    private static BoundExpression BindInlineExpression(BindContext context, InlineExpression syntaxNode)
    {
        return BindExpression(context, syntaxNode.Expression);
    }

    private static BoundNeverExpression BindEmptyExpression(BindContext context, EmptyExpression syntaxNode)
    {
        // TODO: Should this be unit type instead?
        return new BoundNeverExpression(syntaxNode);
    }

    private static BoundExpression BindLiteralExpression(BindContext context, LiteralExpression syntaxNode)
    {
        if (!context.Scope.TryLookup(syntaxNode.Type.Name, out var symbol) || symbol.Type is not TypeType type)
        {
            context.Diagnostics.ReportUndefinedType(syntaxNode.Location, syntaxNode.Type);
            return new BoundNeverExpression(syntaxNode);
        }

        var literal = BindConversion(
            context,
            new BoundLiteralExpression(syntaxNode, syntaxNode.Type, syntaxNode.Value),
            type.Type,
            isExplicit: false);

        return literal;
    }

    private static BoundExpression BindGroupExpression(BindContext context, GroupExpression syntaxNode)
    {
        return BindExpression(context, syntaxNode.Expression);
    }

    private static BoundExpression BindUnaryExpression(BindContext context, UnaryExpression syntaxNode)
    {
        var operand = BindExpression(context, syntaxNode.Operand);
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

    private static BoundExpression BindArgumentListExpression(BindContext context, ArgumentListExpression syntaxNode)
    {
        var boundArguments = new List<BoundExpression>(syntaxNode.Arguments.Count);
        foreach (var argument in syntaxNode.Arguments)
        {
            var boundArgument = BindExpression(context, argument);
            if (boundArgument.Type == PredefinedTypes.Never)
            {
                // Diagnostic already reported as type is BoundNeverExpression.
                return boundArgument;
            }
            boundArguments.Add(boundArgument);
        }

        return new BoundArgumentListExpression(syntaxNode, boundArguments);
    }

    private static BoundExpression BindBinaryExpression(BindContext context, BinaryExpression syntaxNode)
    {
        var operatorKind = syntaxNode.Operator.TokenKind.GetBinaryOperatorKind();

        var left = BindExpression(context, syntaxNode.Left);
        if (left.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return left;
        }

        var right = BindExpression(context, syntaxNode.Right);
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

            var arguments = default(List<BoundExpression>);
            for (int i = 0; i < functionType.Parameters.Count; ++i)
            {
                var argument = BindConversion(
                    context,
                    argumentList.Arguments[i],
                    functionType.Parameters[i].Type,
                    isExplicit: false);

                if (argument.Type == PredefinedTypes.Never)
                {
                    // Diagnostic already reported as type is BoundNeverExpression.
                    return new BoundNeverExpression(syntaxNode);
                }

                if (!ReferenceEquals(argument, argumentList.Arguments[i]))
                    (arguments ??= []).Add(argument);
            }

            if (arguments is not null)
                right = new BoundArgumentListExpression(argumentList.SyntaxNode, arguments);
        }

        var operators = left.Type.Operators
            .Concat(left.Type != right.Type ? right.Type.Operators : Enumerable.Empty<Operator>())
            .Where(op => op.OperatorKind == operatorKind)
            .Cast<BinaryOperator>()
            .Where(op => op.LeftType == left.Type && op.RightType == right.Type)
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

    private static BoundExpression BindDeclarationExpression(BindContext context, DeclarationExpression syntaxNode)
    {
        var symbol = new Symbol(syntaxNode.Identifier.Text.ToString(), syntaxNode.TypeNode.Type, syntaxNode.IsMutable);
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

            expression = BindConversion(
                context,
                BindExpression(context, syntaxNode.Expression),
                functionType.ReturnType,
                isExplicit: false);

            context.PopScope();

            functionType.AddCallOperator();
        }
        else
        {
            expression = BindConversion(
                context,
                BindExpression(context, syntaxNode.Expression),
                symbol.Type,
                isExplicit: false);
        }

        if (expression.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return expression;
        }

        return new BoundDeclarationExpression(syntaxNode, symbol, expression);
    }

    private static BoundExpression BindAssignmentExpression(BindContext context, AssignmentExpression syntaxNode)
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

        var expression = BindConversion(
            context,
            syntaxNode.IsCompound
                ? BindBinaryExpression(context, FromCompoundAssignment(syntaxNode))
                : BindExpression(context, syntaxNode.Expression),
            symbol.Type,
            isExplicit: false);

        if (expression.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return expression;
        }

        return new BoundAssignmentExpression(syntaxNode, symbol, expression);

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

    private static BoundExpression BindNameExpression(BindContext context, NameExpression syntaxNode)
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

    private static BoundExpression BindForExpression(BindContext context, ForExpression syntaxNode)
    {
        throw new NotImplementedException();
    }

    private static BoundExpression BindIfElseExpression(BindContext context, IfElseExpression syntaxNode)
    {
        var condition = BindConversion(
            context,
            BindExpression(context, syntaxNode.Condition),
            PredefinedTypes.Bool,
            isExplicit: false);

        if (condition.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return condition;
        }

        var then = BindExpression(context, syntaxNode.Then);
        if (then.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return then;
        }

        var @else = BindExpression(context, syntaxNode.Else);
        if (@else.Type == PredefinedTypes.Never)
        {
            // Diagnostic already reported as type is BoundNeverExpression.
            return @else;
        }

        return new BoundIfElseExpression(syntaxNode, condition, then, @else);
    }

    private static BoundExpression BindWhileExpression(BindContext context, WhileExpression syntaxNode)
    {
        throw new NotImplementedException();
    }

    private static BoundBreakExpression BindBreakExpression(BindContext context, BreakExpression syntaxNode)
    {
        return new BoundBreakExpression(syntaxNode, BindExpression(context, syntaxNode.Result));
    }

    private static BoundContinueExpression BindContinueExpression(BindContext context, ContinueExpression syntaxNode)
    {
        return new BoundContinueExpression(syntaxNode, BindExpression(context, syntaxNode.Result));
    }

    private static BoundReturnExpression BindReturnExpression(BindContext context, ReturnExpression syntaxNode)
    {
        return new BoundReturnExpression(syntaxNode, BindExpression(context, syntaxNode.Result));
    }

    private static BoundExpression BindConversion(BindContext context, BoundExpression expression, PrimType boundType, bool isExplicit)
    {
        var targetType = boundType is FunctionType f ? f.ReturnType : boundType;
        if (expression.Type == PredefinedTypes.Never || targetType == PredefinedTypes.Never)
            return expression;

        return expression.Type.GetConversion(targetType).Match(
            Identity: () => expression,

            Implicit: @operator => new BoundUnaryExpression(
                expression.SyntaxNode,
                new BoundOperator(expression.SyntaxNode, @operator),
                expression),

            Explicit: @operator =>
            {
                if (!isExplicit)
                {
                    context.Diagnostics.ReportInvalidImplicitConversion(expression.SyntaxNode.Location, expression.Type, targetType);
                    return new BoundNeverExpression(expression.SyntaxNode);
                }

                return new BoundUnaryExpression(
                    expression.SyntaxNode,
                    new BoundOperator(expression.SyntaxNode, @operator),
                    expression);
            },

            None: () =>
            {
                context.Diagnostics.ReportInvalidTypeConversion(expression.SyntaxNode.Location, expression.Type, targetType);
                return new BoundNeverExpression(expression.SyntaxNode);
            });
    }
}

internal delegate TResult BindContextFunc<TState, TResult>(BindContext context, TState state);

internal sealed record class BindContext(Scope Scope, DiagnosticBag Diagnostics)
{
    private readonly Stack<Scope> _stack = new([Scope]);

    public Scope Scope { get => _stack.Peek(); init => _stack = new([value]); }

    public void PushScope() => _stack.Push(Scope.CreateChildScope());

    public void PopScope() => _stack.Pop();
}