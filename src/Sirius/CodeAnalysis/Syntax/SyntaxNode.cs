using Sirius.CodeAnalysis.Text;

namespace Sirius.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }

    public virtual TextSpan Span
    {
        get
        {
            var first = GetChildren().First().Span;
            var last = GetChildren().Last().Span;
            return TextSpan.FromBounds(first.Start, last.End);
        }
    }

    public IEnumerable<SyntaxNode> GetChildren()
    {
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
            {
                SyntaxNode child = (SyntaxNode)property.GetValue(this);
                if (child is not null)
                    yield return child;
            }
            else if (typeof(SeparatedSyntaxList).IsAssignableFrom(property.PropertyType))
            {
                var separatedSynatxList = (SeparatedSyntaxList)property.GetValue(this);
                foreach (var child in separatedSynatxList.GetWithSeparators())
                {
                    yield return child;
                }
            }
            else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
            {
                IEnumerable<SyntaxNode> children = (IEnumerable<SyntaxNode>)property.GetValue(this);
                foreach (var child in children)
                {
                    if (child is not null)
                        yield return child;
                }
            }
        }
    }

    public SyntaxToken GetLastToken()
    {
        if (this is SyntaxToken token)
            return token;

        // A syntax node should always contain at least 1 token.
        return GetChildren().Last().GetLastToken();
    }

    public void WriteTo(TextWriter textWriter) => PrettyPrint(textWriter, this);

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }

    private static void PrettyPrint(TextWriter textWriter, SyntaxNode node, string indent = "", bool isLast = true)
    {
        bool isToConsole = textWriter == Console.Out;
        var marker = isLast ? "└──" : "├──";

        textWriter.Write(indent);

        if (isToConsole)
            Console.ForegroundColor = ConsoleColor.DarkGray;

        textWriter.Write(marker);

        if (isToConsole)
            Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Blue : ConsoleColor.Cyan;

        textWriter.Write(node.Kind);

        if (node is SyntaxToken t && t.Value != null)
        {
            textWriter.Write(" ");
            textWriter.Write(t.Value);
        }

        if (isToConsole)
            Console.ResetColor();

        textWriter.WriteLine();

        indent += isLast ? " " : "│ ";

        SyntaxNode lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
        {
            PrettyPrint(textWriter, child, indent, child == lastChild);
        }
    }
}
