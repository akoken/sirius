using Sirius.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Sirius.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
    public SourceText Text { get; }

    public CompilationUnitSyntax Root { get; }

    public ImmutableArray<Diagnostic> Diagnostics { get; }

    private SyntaxTree(SourceText text)
    {
        Parser parser = new(text);
        var root = parser.ParseCompilationUnit();

        Text = text;
        Root = root;
        Diagnostics = parser.Diagnostics.ToImmutableArray();
    }

    public static SyntaxTree Parse(string text)
    {
        var sourceText = SourceText.From(text);
        return Parse(sourceText);
    }


    public static SyntaxTree Parse(SourceText text)
    {
        return new SyntaxTree(text);
    }

    public static ImmutableArray<SyntaxToken> ParseTokens(string text)
    {
        var sourceText = SourceText.From(text);
        return ParseTokens(sourceText);
    }

    public static ImmutableArray<SyntaxToken> ParseTokens(string text, out ImmutableArray<Diagnostic> diagnostics)
    {
        var sourceText = SourceText.From(text);
        return ParseTokens(sourceText, out diagnostics);
    }

    public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text)
    {
        return ParseTokens(text, out _);
    }

    public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text, out ImmutableArray<Diagnostic> diagnostics)
    {
        IEnumerable<SyntaxToken> LexTokens(Lexer lexer)
        {
            while (true)
            {
                SyntaxToken token = lexer.Lex();
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;

                yield return token;
            }
        }

        Lexer l = new(text);
        ImmutableArray<SyntaxToken> result = LexTokens(l).ToImmutableArray();
        diagnostics = l.Diagnostics.ToImmutableArray();

        return result;
    }
}
