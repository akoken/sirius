using Sirius.CodeAnalysis.Syntax;
using System.Collections.Generic;
using Xunit;

namespace Sirius.Tests.CodeAnalysis.Syntax
{
    public class LexerTest
    {
        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Tokens(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);

            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var token in GetTokens())
            {
                yield return new object[] { token.kind, token.text };
            }
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            return new[]
           {
                (SyntaxKind.PlusToken, "+"),
                (SyntaxKind.MinusToken, "-"),
                (SyntaxKind.StarToken, "*"),
                (SyntaxKind.SlashToken, "/"),
                (SyntaxKind.BangToken, "!"),
                (SyntaxKind.EqualsToken, "="),
                (SyntaxKind.AmpersandAmpersandToken, "&&"),
                (SyntaxKind.PipePipeToken, "||"),
                (SyntaxKind.EqualsEqualsToken, "=="),
                (SyntaxKind.BangEqualsToken, "!="),
                (SyntaxKind.OpenParenthesisToken, "("),
                (SyntaxKind.CloseParenthesisToken, ")"),
                (SyntaxKind.TrueKeyword, "true"),
                (SyntaxKind.FalseKeyword, "false"),
                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abcd"),
                (SyntaxKind.WhitespaceToken, " "),
                (SyntaxKind.WhitespaceToken, "  "),
                (SyntaxKind.WhitespaceToken, "\r"),
                (SyntaxKind.WhitespaceToken, "\n"),
                (SyntaxKind.WhitespaceToken, "\r\n"),
                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "333"),
            };
        }
    }
}