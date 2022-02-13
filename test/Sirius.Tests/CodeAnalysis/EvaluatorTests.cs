using Sirius.CodeAnalysis;
using Sirius.CodeAnalysis.Syntax;
using System.Collections.Generic;
using Xunit;

namespace Sirius.Tests.CodeAnalysis
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("13 + 24", 37)]
        [InlineData("45 - 23", 22)]
        [InlineData("6 * 7", 42)]
        [InlineData("9 / 3", 3)]
        [InlineData("(10)", 10)]
        [InlineData("18 == 4", false)]
        [InlineData("5 == 5", true)]
        [InlineData("18 != 4", true)]
        [InlineData("5 != 5", false)]
        [InlineData("false == false", true)]
        [InlineData("true == false", false)]
        [InlineData("false != false", false)]
        [InlineData("true != false", true)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("(a = 10) * a", 100)]
        public void SyntaxFact_GetText_RoundTrips(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}
