using Sirius.CodeAnalysis.Binding;
using Sirius.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace Sirius.CodeAnalysis;

public class Compilation
{
    private BoundGlobalScope _globalScope;

    public Compilation(SyntaxTree syntaxTree)
        : this(null, syntaxTree)
    {
        SyntaxTree = syntaxTree;
    }

    private Compilation(Compilation previous, SyntaxTree syntaxTree)
    {
        Previous = previous;
        SyntaxTree = syntaxTree;
    }

    public Compilation Previous { get; }
    public SyntaxTree SyntaxTree { get; }

    internal BoundGlobalScope GlobalScope
    {
        get
        {
            if (_globalScope is null)
            {
                var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                Interlocked.CompareExchange(ref _globalScope, globalScope, null);
            }

            return _globalScope;
        }
    }

    public Compilation ContinueWith(SyntaxTree syntaxTree)
    {
        return new Compilation(this, syntaxTree);
    }

    public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
    {
        var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
        if (diagnostics.Length > 0)
            return new EvaluationResult(diagnostics, null);

        var evaluator = new Evaluator(GlobalScope.Statement, variables);
        var value = evaluator.Evaluate();

        return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
    }
}
