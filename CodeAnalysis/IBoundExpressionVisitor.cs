﻿using CodeAnalysis.Binding;

namespace CodeAnalysis;

internal interface IBoundExpressionVisitor<out TResult>
{
    TResult Visit(BoundExpression expression);

    TResult Visit(BoundUnaryExpression expression);

    TResult Visit(BoundBinaryExpression expression);

    TResult Visit(BoundLiteralExpression expression);

    TResult Visit(BoundVariableExpression expression);

    TResult Visit(BoundAssignmentExpression expression);
}
