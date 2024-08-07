﻿using System.Collections.Immutable;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
internal static partial class Lowerer
{
    public static BoundTree Lower(BoundTree boundTree)
    {
        var context = new Context(boundTree.BoundScope);
        var compilationUnit = LowerCompilationUnit(boundTree.CompilationUnit, context);
        compilationUnit = Flatten(compilationUnit);
        return boundTree with { CompilationUnit = compilationUnit };

        static BoundCompilationUnit Flatten(BoundCompilationUnit compilationUnit)
        {
            var nodes = ImmutableArray.CreateBuilder<BoundNode>();
            var stack = new Stack<BoundNode>();
            foreach (var node in compilationUnit.BoundNodes.Reverse())
                stack.Push(node);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current is BoundBlockExpression block)
                {
                    foreach (var expression in block.Expressions.Reverse())
                        stack.Push(expression);
                }
                else
                {
                    nodes.Add(current);
                }
            }

            return new BoundCompilationUnit(compilationUnit.Syntax, new BoundList<BoundNode>(nodes.ToImmutable()));
        }
    }
}
