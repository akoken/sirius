﻿using System.Collections.Immutable;
using Sirius.CodeAnalysis.Symbols;

namespace Sirius.CodeAnalysis.Binding;

internal sealed class BoundScope
{
    private Dictionary<string, VariableSymbol> _variables;
    private Dictionary<string, FunctionSymbol> _functions;

    public BoundScope Parent { get; }

    public BoundScope(BoundScope parent)
    {
        Parent = parent;
    }

    public bool TryDeclareVariable(VariableSymbol variable)
    {
        if (_variables is null)
            _variables = new Dictionary<string, VariableSymbol>();

        if (_variables.ContainsKey(variable.Name))
            return false;

        _variables.Add(variable.Name, variable);

        return true;
    }

    public bool TryLookUpVariable(string name, out VariableSymbol variable)
    {
        variable = null;

        if (_variables is not null && _variables.TryGetValue(name, out variable))
            return true;

        if (Parent is null)
            return false;

        return Parent.TryLookUpVariable(name, out variable);
    }

    public bool TryDeclareFunction(FunctionSymbol function)
    {
        if (_functions is null)
            _functions = new Dictionary<string, FunctionSymbol>();

        if (_functions.ContainsKey(function.Name))
            return false;

        _functions.Add(function.Name, function);

        return true;
    }

    public bool TryLookUpFunction(string name, out FunctionSymbol function)
    {
        function = null;

        if (_functions is not null && _functions.TryGetValue(name, out function))
            return true;

        if (Parent is null)
            return false;

        return Parent.TryLookUpFunction(name, out function);
    }

    public ImmutableArray<VariableSymbol> GetDeclaredVariables()
    {
        if (_variables is null)
            return ImmutableArray<VariableSymbol>.Empty;

        return _variables.Values.ToImmutableArray();
    }

    public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
    {
        if (_functions is null)
            return ImmutableArray<FunctionSymbol>.Empty;

        return _functions.Values.ToImmutableArray();
    }
}
