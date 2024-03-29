﻿using Sirius.CodeAnalysis;
using Sirius.CodeAnalysis.Symbols;
using Sirius.CodeAnalysis.Syntax;
using Xunit;

namespace Sirius.Tests.CodeAnalysis;

public class EvaluatorTests
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("+1", 1)]
    [InlineData("-1", -1)]
    [InlineData("~1", -2)]
    [InlineData("13 + 24", 37)]
    [InlineData("45 - 23", 22)]
    [InlineData("6 * 7", 42)]
    [InlineData("9 / 3", 3)]
    [InlineData("(10)", 10)]
    [InlineData("18 == 4", false)]
    [InlineData("5 == 5", true)]
    [InlineData("18 != 4", true)]
    [InlineData("5 != 5", false)]
    [InlineData("5 < 8", true)]
    [InlineData("9 < 6", false)]
    [InlineData("5 <= 8", true)]
    [InlineData("5 <= 5", true)]
    [InlineData("9 <= 6", false)]
    [InlineData("5 > 3", true)]
    [InlineData("9 > 12", false)]
    [InlineData("35 >= 8", true)]
    [InlineData("5 >= 5", true)]
    [InlineData("1 >= 6", false)]
    [InlineData("1 | 2", 3)]
    [InlineData("1 | 0", 1)]
    [InlineData("1 & 3", 1)]
    [InlineData("1 & 0", 0)]
    [InlineData("1 ^ 0", 1)]
    [InlineData("0 ^ 1", 1)]
    [InlineData("1 ^ 3", 2)]
    [InlineData("false == false", true)]
    [InlineData("true == false", false)]
    [InlineData("false != false", false)]
    [InlineData("true != false", true)]
    [InlineData("true && true", true)]
    [InlineData("false || false", false)]
    [InlineData("true && false", false)]
    [InlineData("false || true", true)]
    [InlineData("false | false", false)]
    [InlineData("false | true", true)]
    [InlineData("true | false", true)]
    [InlineData("true | true", true)]
    [InlineData("false & false", false)]
    [InlineData("false & true", false)]
    [InlineData("true & false", false)]
    [InlineData("true & true", true)]
    [InlineData("false ^ false", false)]
    [InlineData("true ^ false", true)]
    [InlineData("false ^ true", true)]
    [InlineData("true ^ true", false)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("!true", false)]
    [InlineData("!false", true)]
    [InlineData("var a = 10", 10)]
    [InlineData("{ var a = 10 (a * a) }", 100)]
    [InlineData("{ var a = 0 (a = 10) * a }", 100)]
    [InlineData("{ var a = 0 if a == 0 a = 10 a }", 10)]
    [InlineData("{ var a = 0 if a == 5 a = 10 a }", 0)]
    [InlineData("{ var a = 0 if a == 0 a = 10 else a = 5 a }", 10)]
    [InlineData("{ var a = 0 if a == 5 a = 10 else a = 6 a }", 6)]
    [InlineData("{ var i = 10 var result = 0 while i > 0 { result = result + i i = i - 1} result }", 55)]
    [InlineData("{ var result = 0 for i = 1 to 10 { result = result + i } result }", 55)]
    [InlineData("{ var a = 10 for i = 1 to (a = a - 1) { } a }", 9)]
    public void Evaluator_Computes_CorrectValues(string text, object expectedValue)
    {
        AssertValue(text, expectedValue);
    }

    [Fact]
    public void Evaluator_VariableDeclaration_Reports_Redeclaration()
    {
        var text = @"
                {
                    var x = 10
                    var y = 100
                    {
                        var x = 10
                    }
                    var [x] = 5
                }
            ";

        var diagnostics = @"
                'x' is already declared.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_Variables_Can_Shadow_Functions()
    {
        var text = @"
                {
                    let print = 42
                    [print](""test"")
                }
            ";

        var diagnostics = @"
                Function 'print' does not exist.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_BlockStatement_NoInfiniteLoop()
    {
        var text = @"
                {
                [)][]
            ";

        var diagnostics = @"
                Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>.
                Unexpected token <EndOfFileToken>, expected <CloseBraceToken>.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_IfStatement_Reports_CannotConvert()
    {
        var text = @"
                {
                    var x = 0
                    if [10]
                        x = 10
                }
            ";

        var diagnostics = @"
                Cannot convert type 'int' to 'bool'.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_WhileStatement_Reports_CannotConvert()
    {
        var text = @"
                {
                    var x = 0
                    while [10]
                        x = 10
                }
            ";

        var diagnostics = @"
                Cannot convert type 'int' to 'bool'.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_DoWhileStatement_Reports_CannotConvert()
    {
        var text = @"
                {
                    var x = 0
                    do
                        x = 10
                    while [10]
                }
            ";

        var diagnostics = @"
                Cannot convert type 'int' to 'bool'.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_ForStatement_Reports_CannotConvert_LowerBound()
    {
        var text = @"
                {
                    var result = 0
                    for i = [false] to 10
                        result = result + 1
                }
            ";

        var diagnostics = @"
                Cannot convert type 'bool' to 'int'.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_ForStatement_Reports_CannotConvert_UpperBound()
    {
        var text = @"
                {
                    var result = 0
                    for i = 1 to [true]
                        result = result + 1
                }
            ";

        var diagnostics = @"
                Cannot convert type 'bool' to 'int'.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_NameExpression_Reports_Undefined()
    {
        var text = @"[x] * 10";

        var diagnostics = @"
                Variable 'x' does not exist.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_NameExpression_Reports_NoErrorForInsertedToken()
    {
        var text = @"[]";

        var diagnostics = @"
                Unexpected token <EndOfFileToken>, expected <IdentifierToken>.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_UnaryExpression_Reports_Undefined()
    {
        var text = @"[+]true";

        var diagnostics = @"
                Unary operator '+' is not defined for type 'bool'.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_BinaryExpression_Reports_Undefined()
    {
        var text = @"10 [*] false";

        var diagnostics = @"
                Binary operator '*' is not defined for types 'int' and 'bool'.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_AssignmentExpression_Reports_Undefined()
    {
        var text = @"[x] = 10";

        var diagnostics = @"
                Variable 'x' does not exist.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_AssignmentExpression_Reports_CannotAssign()
    {
        var text = @"
                {
                    let x = 10
                    x [=] 0
                }
            ";

        var diagnostics = @"
                Variable 'x' is read-only and cannot be assigned to.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void Evaluator_AssignmentExpression_Reports_CannotConvert()
    {
        var text = @"
                {
                    var x = 10
                    x = [true]
                }
            ";

        var diagnostics = @"
                Cannot convert type 'bool' to 'int'.
            ";

        AssertDiagnostics(text, diagnostics);
    }

    private static void AssertValue(string text, object expectedValue)
    {
        var syntaxTree = SyntaxTree.Parse(text);
        var compilation = new Compilation(syntaxTree);
        var variables = new Dictionary<VariableSymbol, object>();
        var result = compilation.Evaluate(variables);

        Assert.Empty(result.Diagnostics);
        Assert.Equal(expectedValue, result.Value);
    }

    private void AssertDiagnostics(string text, string diagnosticText)
    {
        var annotatedText = AnnotatedText.Parse(text);
        var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
        var compilation = new Compilation(syntaxTree);
        var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

        var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

        if (annotatedText.Spans.Length != expectedDiagnostics.Length)
            throw new Exception("ERROR: Must mark as many spans as there are expected diagnostics");

        Assert.Equal(expectedDiagnostics.Length, result.Diagnostics.Length);

        for (var i = 0; i < expectedDiagnostics.Length; i++)
        {
            var expectedMessage = expectedDiagnostics[i];
            var actualMessage = result.Diagnostics[i].Message;
            Assert.Equal(expectedMessage, actualMessage);

            var expectedSpan = annotatedText.Spans[i];
            var actualSpan = result.Diagnostics[i].Span;
            Assert.Equal(expectedSpan, actualSpan);
        }
    }
}
