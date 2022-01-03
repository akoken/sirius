﻿namespace Sirius.CodeAnalysis;

internal sealed class Parser
{
    private readonly SyntaxToken[] _tokens;

    private int _position;

    private List<string> _diagnostics = new();

    public Parser(string text)
    {
        List<SyntaxToken> tokens = new();
        Lexer lexer = new(text);
        SyntaxToken token;

        do
        {
            token = lexer.Lex();
            if (token.Kind != SyntaxKind.WhitespaceToken && token.Kind != SyntaxKind.InvalidToken)
            {
                tokens.Add(token);
            }
        } while (token.Kind != SyntaxKind.EndOfFileToken);

        _tokens = tokens.ToArray();
        _diagnostics.AddRange(lexer.Diagnostics);
    }

    public IEnumerable<string> Diagnostics => _diagnostics;

    private SyntaxToken Current => Peek(0);

    public SyntaxTree Parse()
    {
        ExpressionSyntax expression = ParseExpression();
        SyntaxToken endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

        return new SyntaxTree(expression, endOfFileToken, _diagnostics);
    }

    private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left = ParsePrimaryExpression();
        while (true)
        {
            int precedence = GetBinaryOperatorPrecedence(Current.Kind);
            if (precedence == 0 || precedence <= parentPrecedence)
                break;

            SyntaxToken operatorToken = NextToken();
            ExpressionSyntax right = ParseExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private static int GetBinaryOperatorPrecedence(SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.StarToken:
            case SyntaxKind.SlashToken:
                return 2;
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
                return 1;
            default:
                return 0;
        }
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        if (Current.Kind == SyntaxKind.OpenParenthesisToken)
        {
            SyntaxToken left = NextToken();
            ExpressionSyntax expression = ParseExpression();
            SyntaxToken right = MatchToken(SyntaxKind.CloseParenthesisToken);

            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        SyntaxToken numberToken = MatchToken(SyntaxKind.NumberToken);

        return new LiteralExpressionSyntax(numberToken);
    }

    private SyntaxToken Peek(int offset = 0)
    {
        int index = _position + offset;
        if (index >= _tokens.Length)
        {
            return _tokens[_tokens.Length - 1];
        }

        return _tokens[index];
    }

    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
        {
            return NextToken();
        }

        _diagnostics.Add($"Error: Unexpected token <{Current.Kind}>, expected <{kind}>");

        return new SyntaxToken(kind, Current.Position, null, null);
    }
}
