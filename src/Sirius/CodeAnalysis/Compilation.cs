using Sirius.CodeAnalysis.Binding;
using Sirius.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace Sirius.CodeAnalysis;

public class Compilation
{
    public Compilation(SyntaxTree syntaxTree)
    {
        SyntaxTree = syntaxTree;
    }

    public SyntaxTree SyntaxTree { get; }

    public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
    {
        var binder = new Binder(variables);
        var boundExpression = binder.BindExpression(SyntaxTree.Root.Expression);

        var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToImmutableArray();
        if (diagnostics.Length > 0)
            return new EvaluationResult(diagnostics, null);

        var evaluator = new Evaluator(boundExpression, variables);
        var value = evaluator.Evaluate();

        return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
    }
}
