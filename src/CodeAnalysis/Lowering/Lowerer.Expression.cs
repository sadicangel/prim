﻿using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundExpression LowerExpression(BoundExpression node, Context context)
    {
        return node.BoundKind switch
        {
            BoundKind.NopExpression => LowerNopExpression((BoundNopExpression)node, context),
            BoundKind.NeverExpression => LowerNeverExpression((BoundNeverExpression)node, context),
            BoundKind.StackInstantiation => LowerStackInstantiation((BoundStackInstantiation)node, context),
            BoundKind.LiteralExpression => LowerLiteralExpression((BoundLiteralExpression)node, context),
            BoundKind.AssignmentExpression => LowerAssignmentExpression((BoundAssignmentExpression)node, context),
            BoundKind.ModuleDeclaration => LowerModuleDeclaration((BoundModuleDeclaration)node, context),
            BoundKind.LabelDeclaration => LowerLabelDeclaration((BoundLabelDeclaration)node, context),
            BoundKind.StructDeclaration => LowerStructDeclaration((BoundStructDeclaration)node, context),
            BoundKind.VariableDeclaration => LowerVariableDeclaration((BoundVariableDeclaration)node, context),
            BoundKind.LocalReference => LowerLocalReference((BoundLocalReference)node, context),
            BoundKind.GlobalReference => LowerGlobalReference((BoundGlobalReference)node, context),
            BoundKind.PropertyReference => LowerPropertyReference((BoundPropertyReference)node, context),
            BoundKind.MethodReference => LowerMethodReference((BoundMethodReference)node, context),
            BoundKind.MethodGroup => LowerMethodGroup((BoundMethodGroup)node, context),
            BoundKind.IndexReference => LowerIndexReference((BoundIndexReference)node, context),
            BoundKind.BlockExpression => LowerBlockExpression((BoundBlockExpression)node, context),
            BoundKind.ArrayInitExpression => LowerArrayInitExpression((BoundArrayInitExpression)node, context),
            BoundKind.StructInitExpression => LowerStructInitExpression((BoundStructInitExpression)node, context),
            BoundKind.InvocationExpression => LowerInvocationExpression((BoundInvocationExpression)node, context),
            BoundKind.ConversionExpression => LowerConversionExpression((BoundConversionExpression)node, context),
            BoundKind.UnaryExpression => LowerUnaryExpression((BoundUnaryExpression)node, context),
            BoundKind.BinaryExpression => LowerBinaryExpression((BoundBinaryExpression)node, context),
            BoundKind.IfExpression => LowerIfExpression((BoundIfExpression)node, context),
            BoundKind.WhileExpression => LowerWhileExpression((BoundWhileExpression)node, context),
            BoundKind.ContinueExpression => LowerContinueExpression((BoundContinueExpression)node, context),
            BoundKind.BreakExpression => LowerBreakExpression((BoundBreakExpression)node, context),
            BoundKind.ReturnExpression => LowerReturnExpression((BoundReturnExpression)node, context),
            BoundKind.GotoExpression => LowerGotoExpression((BoundGotoExpression)node, context),
            BoundKind.ConditionalGotoExpression => LowerConditionalGotoExpression((BoundConditionalGotoExpression)node, context),
            _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'"),
        };
    }
}
