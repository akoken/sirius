namespace Sirius.CodeAnalysis;

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
            token = lexer.NextToken();
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

    private ExpressionSyntax ParseExpression()
    {
        return ParseTerm();
    }

    private ExpressionSyntax ParseTerm()
    {
        ExpressionSyntax left = ParseFactor();

        while (Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken)
        {
            SyntaxToken operatorToken = NextToken();
            ExpressionSyntax right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParseFactor()
    {
        ExpressionSyntax left = ParsePrimaryExpression();

        while (Current.Kind == SyntaxKind.StarToken || Current.Kind == SyntaxKind.SlashToken)
        {
            SyntaxToken operatorToken = NextToken();
            ExpressionSyntax right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
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
