using Sirius.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Sirius.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
    public SourceText Text { get; }
    public ExpressionSyntax Root { get; }

    public SyntaxToken EndOfFileToken { get; }

    public ImmutableArray<Diagnostic> Diagnostics { get; }

    public SyntaxTree(SourceText text, ExpressionSyntax root, SyntaxToken endOfFileToken, ImmutableArray<Diagnostic> diagnostics)
    {
        Text = text;
        Root = root;
        EndOfFileToken = endOfFileToken;
        Diagnostics = diagnostics;
    }

    public static SyntaxTree Parse(string text)
    {
        var sourceText = SourceText.From(text);
        return Parse(sourceText);
    }

    public static SyntaxTree Parse(SourceText text)
    {
        Parser parser = new(text);
        return parser.Parse();
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
