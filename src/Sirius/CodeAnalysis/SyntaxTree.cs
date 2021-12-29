namespace Sirius.CodeAnalysis;

sealed class SyntaxTree
{
    public ExpressionSyntax Root { get; }

    public SyntaxToken EndOfFileToken { get; }

    public IReadOnlyList<string> Diagnostics { get; }

    public SyntaxTree(ExpressionSyntax root, SyntaxToken endOfFileToken, IEnumerable<string> diagnostics)
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
