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
        var diagnostics = parser.Diagnostics.ToImmutableArray();

        Text = text;
        Root = root;
        Diagnostics = diagnostics;
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

    public static IEnumerable<SyntaxToken> ParseTokens(string text)
    {
        var sourceText = SourceText.From(text);
        return ParseTokens(sourceText);
    }

    public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
    {
        Lexer lexer = new(text);
        while (true)
        {
            SyntaxToken token = lexer.Lex();
            if (token.Kind == SyntaxKind.EndOfFileToken)
                break;

            yield return token;
        }
    }
}
