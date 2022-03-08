﻿using System.Collections.Immutable;

namespace Sirius.CodeAnalysis.Binding;

internal sealed class BoundScope
{
    private Dictionary<string, VariableSymbol> _variables = new();

    public BoundScope Parent { get; }

    public BoundScope(BoundScope parent)
    {
        Parent = parent;
    }

    public bool TryDeclare(VariableSymbol variable)
    {
        if (_variables.ContainsKey(variable.Name))
            return false;

        _variables.Add(variable.Name, variable);
        return true;
    }

    public bool TryLookUp(string name, out VariableSymbol variable)
    {
        if (_variables.TryGetValue(name, out variable))
            return true;

        if (Parent is null)
            return false;

        return Parent.TryLookUp(name, out variable);
    }

    public ImmutableArray<VariableSymbol> GetDeclaredVariables()
    {
        return _variables.Values.ToImmutableArray();
    }
}
