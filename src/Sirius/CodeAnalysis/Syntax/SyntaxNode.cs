namespace Sirius.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }

    public IEnumerable<SyntaxNode> GetChildren()
    {
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
            {
                SyntaxNode child = (SyntaxNode)property.GetValue(this);
                yield return child;
            }
            else if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
            {
                IEnumerable<SyntaxNode> children = (IEnumerable<SyntaxNode>)property.GetValue(this);
                foreach (var child in children)
                {
                    yield return child;
                }
            }
        }
    }
}
