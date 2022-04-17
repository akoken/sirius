﻿using System.Collections;
using Sirius.CodeAnalysis.Symbols;
using Sirius.CodeAnalysis.Syntax;
using Sirius.CodeAnalysis.Text;

namespace Sirius.CodeAnalysis;

internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = new();

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddRange(DiagnosticBag diagnostics)
    {
        _diagnostics.AddRange(diagnostics._diagnostics);
    }

    public void ReportInvalidNumber(TextSpan span, string text, TypeSymbol type)
    {
        var message = $"The number {text} isn't a valid {type}.";
        Report(span, message);
    }

    public void ReportBadCharacter(int position, char character)
    {
        var span = new TextSpan(position, 1);
        var message = $"Bad character input: '{character}'.";
        Report(span, message);
    }

    public void ReportUnterminatedString(TextSpan span)
    {
        var message = "Unterminated string literal.";
        Report(span, message);
    }

    public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
    {
        var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
        Report(span, message);
    }

    public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
    {
        var message = $"Unary operator '{operatorText}' is not defined for type '{operandType}'.";
        Report(span, message);
    }

    public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
    {
        var message = $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.";
        Report(span, message);
    }

    private void Report(TextSpan span, string message)
    {
        var diagnostic = new Diagnostic(span, message);
        _diagnostics.Add(diagnostic);
    }

    public void ReportUndefinedName(TextSpan span, string name)
    {
        var message = $"Variable '{name}' does not exist.";
        Report(span, message);
    }

    public void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
    {
        var message = $"Cannot convert type '{fromType}' to '{toType}'.";
        Report(span, message);
    }

    public void ReportVariableAlreadyDeclared(TextSpan span, string name)
    {
        var message = $"Variable '{name}' is already declared.";
        Report(span, message);
    }

    public void ReportCanNotAssign(TextSpan span, string name)
    {
        var message = $"Variable '{name}' is read-only and cannot be assigned to.";
        Report(span, message);
    }
}
