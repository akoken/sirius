namespace Sirius.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
    public ExpressionSyntax Root { get; }

    public SyntaxToken EndOfFileToken { get; }

    public IReadOnlyList<Diagnostic> Diagnostics { get; }

    public SyntaxTree(ExpressionSyntax root, SyntaxToken endOfFileToken, IEnumerable<Diagnostic> diagnostics)
    {
        Root = root;
        EndOfFileToken = endOfFileToken;
        Diagnostics = diagnostics.ToArray();
    }

    public static SyntaxTree Parse(string text)
    {
        Parser parser = new(text);
        return parser.Parse();
    }
}
