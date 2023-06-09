﻿using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Binding;

internal interface IBoundExpressionVisitor<out TResult>
{
    TResult Visit(BoundNeverExpression expression);
    TResult Visit(BoundUnaryExpression expression);
    TResult Visit(BoundBinaryExpression expression);
    TResult Visit(BoundLiteralExpression expression);
    TResult Visit(BoundSymbolExpression expression);
    TResult Visit(BoundAssignmentExpression expression);
    TResult Visit(BoundIfExpression expression);
    TResult Visit(BoundCallExpression expression);
    TResult Visit(BoundConvertExpression expression);
}
