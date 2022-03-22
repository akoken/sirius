using Sirius.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sirius.Tests.CodeAnalysis.Syntax;

public class LexerTests
{
    [Fact]
    public void Lexer_Tests_AllTokens()
    {
        var tokenKinds = Enum.GetValues(typeof(SyntaxKind))
            .Cast<SyntaxKind>()
            .Where(x => x.ToString().EndsWith("Keyword") || x.ToString().EndsWith("Token")).ToList();

        var testedTokenKinds = GetTokens().Concat(GetSeperators()).Select(x => x.kind);

        var untestedTokenKinds = new SortedSet<SyntaxKind>(tokenKinds);
        untestedTokenKinds.Remove(SyntaxKind.InvalidToken);
        untestedTokenKinds.Remove(SyntaxKind.EndOfFileToken);
        untestedTokenKinds.ExceptWith(testedTokenKinds);

        Assert.Empty(untestedTokenKinds);
    }

    [Theory]
    [MemberData(nameof(GetTokensData))]
    public void Lexer_Lexes_Tokens(SyntaxKind kind, string text)
    {
        var tokens = SyntaxTree.ParseTokens(text);

        var token = Assert.Single(tokens);
        Assert.Equal(kind, token.Kind);
        Assert.Equal(text, token.Text);
    }

    [Theory]
    [MemberData(nameof(GetTokenPairsData))]
    public void Lexer_Lexes_TokenPairs(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)
    {
        var text = $"{t1Text}{t2Text}";
        var tokens = SyntaxTree.ParseTokens(text).ToArray();

        Assert.Equal(2, tokens.Length);

        Assert.Equal(t1Kind, tokens[0].Kind);
        Assert.Equal(t1Text, tokens[0].Text);
        Assert.Equal(t2Kind, tokens[1].Kind);
        Assert.Equal(t2Text, tokens[1].Text);
    }

    [Theory]
    [MemberData(nameof(GetTokenPairsWithSeperatorData))]
    public void Lexer_Lexes_TokenPairsWithSeperator(SyntaxKind t1Kind, string t1Text, SyntaxKind seperatorKind, string seperatorText, SyntaxKind t2Kind, string t2Text)
    {
        var text = $"{t1Text}{seperatorText}{t2Text}";
        var tokens = SyntaxTree.ParseTokens(text).ToArray();

        Assert.Equal(3, tokens.Length);

        Assert.Equal(t1Kind, tokens[0].Kind);
        Assert.Equal(t1Text, tokens[0].Text);
        Assert.Equal(seperatorKind, tokens[1].Kind);
        Assert.Equal(seperatorText, tokens[1].Text);
        Assert.Equal(t2Kind, tokens[2].Kind);
        Assert.Equal(t2Text, tokens[2].Text);
    }

    public static IEnumerable<object[]> GetTokensData()
    {
        foreach (var token in GetTokens().Concat(GetSeperators()))
        {
            yield return new object[] { token.kind, token.text };
        }
    }

    public static IEnumerable<object[]> GetTokenPairsData()
    {
        foreach (var token in GetTokenPairs())
        {
            yield return new object[] { token.t1Kind, token.t1Text, token.t2Kind, token.t2Text };
        }
    }

    public static IEnumerable<object[]> GetTokenPairsWithSeperatorData()
    {
        foreach (var token in GetTokenPairsWithSeperator())
        {
            yield return new object[] { token.t1Kind, token.t1Text, token.seperatorKind, token.seperatorText, token.t2Kind, token.t2Text };
        }
    }

    private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
    {
        var fixedTokens = Enum.GetValues(typeof(SyntaxKind))
            .Cast<SyntaxKind>()
            .Select(k => (kind: k, text: SyntaxFacts.GetText(k)))
            .Where(t => t.text != null);

        var dynamicTokens = new[]
        {
            (SyntaxKind.IdentifierToken, "a"),
            (SyntaxKind.IdentifierToken, "abcd"),
            (SyntaxKind.NumberToken, "1"),
            (SyntaxKind.NumberToken, "333"),
        };

        return fixedTokens.Concat(dynamicTokens);
    }

    private static IEnumerable<(SyntaxKind kind, string text)> GetSeperators()
    {
        return new[]
       {
            (SyntaxKind.WhitespaceToken, " "),
            (SyntaxKind.WhitespaceToken, "  "),
            (SyntaxKind.WhitespaceToken, "\r"),
            (SyntaxKind.WhitespaceToken, "\n"),
            (SyntaxKind.WhitespaceToken, "\r\n")
        };
    }

    private static bool RequiresSeperator(SyntaxKind t1Kind, SyntaxKind t2Kind)
    {
        var t1IsKeyword = t1Kind.ToString().EndsWith("Keyword");
        var t2IsKeyword = t2Kind.ToString().EndsWith("Keyword");

        if (t1Kind == SyntaxKind.IdentifierToken && t2Kind == SyntaxKind.IdentifierToken)
            return true;

        if (t1Kind == SyntaxKind.NumberToken && t2Kind == SyntaxKind.NumberToken)
            return true;

        if (t1Kind == SyntaxKind.BangToken && t2Kind == SyntaxKind.EqualsToken)
            return true;

        if (t1Kind == SyntaxKind.BangToken && t2Kind == SyntaxKind.EqualsEqualsToken)
            return true;

        if (t1Kind == SyntaxKind.EqualsToken && t2Kind == SyntaxKind.EqualsToken)
            return true;

        if (t1Kind == SyntaxKind.EqualsToken && t2Kind == SyntaxKind.EqualsEqualsToken)
            return true;

        if (t1Kind == SyntaxKind.LessToken && t2Kind == SyntaxKind.EqualsToken)
            return true;

        if (t1Kind == SyntaxKind.LessToken && t2Kind == SyntaxKind.EqualsEqualsToken)
            return true;

        if (t1Kind == SyntaxKind.GreaterToken && t2Kind == SyntaxKind.EqualsToken)
            return true;

        if (t1Kind == SyntaxKind.GreaterToken && t2Kind == SyntaxKind.EqualsEqualsToken)
            return true;

        if (t1IsKeyword && t2IsKeyword)
            return true;

        if (t1IsKeyword && t2Kind == SyntaxKind.IdentifierToken)
            return true;

        if (t2IsKeyword && t1Kind == SyntaxKind.IdentifierToken)
            return true;

        return false;
    }

    private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairs()
    {
        foreach (var t1 in GetTokens())
        {
            foreach (var t2 in GetTokens())
            {
                if (!RequiresSeperator(t1.kind, t2.kind))
                    yield return (t1.kind, t1.text, t2.kind, t2.text);
            }
        }
    }

    private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind seperatorKind, string seperatorText, SyntaxKind t2Kind, string t2Text)> GetTokenPairsWithSeperator()
    {
        foreach (var t1 in GetTokens())
        {
            foreach (var t2 in GetTokens())
            {
                if (RequiresSeperator(t1.kind, t2.kind))
                {
                    foreach (var s in GetSeperators())
                    {
                        yield return (t1.kind, t1.text, s.kind, s.text, t2.kind, t2.text);
                    }
                }
            }
        }
    }
}
