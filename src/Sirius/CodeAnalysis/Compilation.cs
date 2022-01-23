﻿using Sirius.CodeAnalysis.Binding;
using Sirius.CodeAnalysis.Syntax;

namespace Sirius.CodeAnalysis;

public class Compilation
{
    public Compilation(SyntaxTree syntaxTree)
    {
        SyntaxTree = syntaxTree;
    }

    public SyntaxTree SyntaxTree { get; }

    public EvaluationResult Evaluate()
    {
        var binder = new Binder();
        var boundExpression = binder.BindExpression(SyntaxTree.Root);

        var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();
        if (diagnostics.Length > 0)
            return new EvaluationResult(diagnostics, null);

        var evaluator = new Evaluator(boundExpression);
        var value = evaluator.Evaluate();

        return new EvaluationResult(Array.Empty<Diagnostic>(), value);
    }
}