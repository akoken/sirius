// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Sirius.CodeAnalysis.Syntax;

public sealed class ParameterSyntax : SyntaxNode
{
    public ParameterSyntax(SyntaxToken identifier, TypeClauseSyntax type)
    {
        Identifier = identifier;
        Type = type;
    }

    public override SyntaxKind Kind => SyntaxKind.Parameter;

    public SyntaxToken Identifier { get; }
    public TypeClauseSyntax Type { get; }
}
