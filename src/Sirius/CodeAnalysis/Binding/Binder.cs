﻿using Sirius.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace Sirius.CodeAnalysis.Binding;

internal sealed class Binder
{
    private readonly DiagnosticBag _diagnostics = new();

    private BoundScope _boundScope;

    public Binder(BoundScope parent)
    {
        _boundScope = new BoundScope(parent);
    }

    public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
    {
        var parentScope = CreateParentScopes(previous);
        var binder = new Binder(parentScope);
        var expression = binder.BindExpression(syntax.Expression);
        var variables = binder._boundScope.GetDeclaredVariables();
        var diagnostics = binder.Diagnostics.ToImmutableArray();

        if (previous is not null)
        {
            diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);
        }

        return new BoundGlobalScope(previous, diagnostics, variables, expression);
    }

    private static BoundScope CreateParentScopes(BoundGlobalScope previous)
    {
        var stack = new Stack<BoundGlobalScope>();

        while (previous is not null)
        {
            stack.Push(previous);
            previous = previous.Previous;
        }

        BoundScope parent = null;
        while (stack.Count > 0)
        {
            previous = stack.Pop();
            var scope = new BoundScope(parent);

            foreach (var v in previous.Variables)
            {
                scope.TryDeclare(v);
            }

            parent = scope;
        }

        return parent;
    }

    public DiagnosticBag Diagnostics => _diagnostics;

    public BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        switch (syntax.Kind)
        {
            case SyntaxKind.ParenthesizedExpression:
                return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
            case SyntaxKind.LiteralExpression:
                return BindLiteralExpression((LiteralExpressionSyntax)syntax);
            case SyntaxKind.NameExpression:
                return BindNameExpression((NameExpressionSyntax)syntax);
            case SyntaxKind.AssignmentExpression:
                return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
            case SyntaxKind.UnaryExpression:
                return BindUnaryExpression((UnaryExpressionSyntax)syntax);
            case SyntaxKind.BinaryExpression:
                return BindBinaryExpression((BinaryExpressionSyntax)syntax);
            default:
                throw new Exception($"Unexpected syntax {syntax.Kind}");
        }
    }

    private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
    {
        return BindExpression(syntax.Expression);
    }

    private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var boundOperand = BindExpression(syntax.Operand);
        var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

        if (boundOperator is null)
        {
            _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
            return boundOperand;
        }

        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

        if (boundOperator == null)
        {
            _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
            return boundLeft;
        }

        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }

    private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }

    private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text;

        if (!_boundScope.TryLookUp(name, out var variable))
        {
            _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
            return new BoundLiteralExpression(0);
        }

        return new BoundVariableExpression(variable);
    }

    private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text;
        var boundExpression = BindExpression(syntax.Expression);

        if (!_boundScope.TryLookUp(name, out var variable))
        {
            variable = new VariableSymbol(name, boundExpression.Type);
            _boundScope.TryDeclare(variable);
        }

        if (boundExpression.Type != variable.Type)
        {
            _diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
            return boundExpression;
        }

        return new BoundAssignmentExpression(variable, boundExpression);
    }
}
