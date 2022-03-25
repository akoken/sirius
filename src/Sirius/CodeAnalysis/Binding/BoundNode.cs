namespace Sirius.CodeAnalysis.Binding;

internal abstract class BoundNode
{
    public abstract BoundNodeKind Kind { get; }

    public IEnumerable<BoundNode> GetChildren()
    {
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
            {
                BoundNode child = (BoundNode)property.GetValue(this);
                if (child is not null)
                    yield return child;
            }
            else if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
            {
                IEnumerable<BoundNode> children = (IEnumerable<BoundNode>)property.GetValue(this);
                foreach (var child in children)
                {
                    if (child is not null)
                        yield return child;
                }
            }
        }
    }

    public void WriteTo(TextWriter textWriter) => PrettyPrint(textWriter, this);

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }

    private static void PrettyPrint(TextWriter textWriter, BoundNode node, string indent = "", bool isLast = true)
    {
        bool isToConsole = textWriter == Console.Out;
        var marker = isLast ? "└──" : "├──";

        if (isToConsole)
            Console.ForegroundColor = ConsoleColor.DarkGray;

        textWriter.Write(indent);
        textWriter.Write(marker);

        WriteNode(textWriter, node);

        if (isToConsole)
            Console.ResetColor();

        textWriter.WriteLine();

        indent += isLast ? " " : "│ ";

        BoundNode lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
        {
            PrettyPrint(textWriter, child, indent, child == lastChild);
        }
    }

    private static void WriteNode(TextWriter writer, BoundNode node)
    {
        Console.ForegroundColor = GetColor(node);
        writer.Write(node.Kind);
        Console.ResetColor();
    }

    private static ConsoleColor GetColor(BoundNode node)
    {
        if (node is BoundExpression)
            return ConsoleColor.Blue;

        if (node is BoundStatement)
            return ConsoleColor.Cyan;

        return ConsoleColor.Yellow;
    }
}