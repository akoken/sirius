namespace Sirius.CodeAnalysis.Syntax;

internal sealed class Lexer
{
    private readonly string _text;
    private int _position;
    private List<string> _diagnostics = new();

    private char Current
    {
        get
        {
            if (_position >= _text.Length)
            {
                return '\0';
            }

            return _text[_position];
        }
    }

    public IEnumerable<string> Diagnostics => _diagnostics;

    public Lexer(string text)
    {
        _text = text;
    }

    public SyntaxToken Lex()
    {
        if (_position >= _text.Length)
        {
            return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
        }

        if (char.IsDigit(Current))
        {
            int start = _position;

            while (char.IsDigit(Current))
            {
                Next();
            }

            int length = _position - start;
            string text = _text.Substring(start, length);
            if (!int.TryParse(text, out int value))
            {
                _diagnostics.Add($"The number {_text} is not valid Int32.");
            }

            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }

        if (char.IsWhiteSpace(Current))
        {
            int start = _position;

            while (char.IsWhiteSpace(Current))
            {
                Next();
            }

            int length = _position - start;
            string text = _text.Substring(start, length);

            return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
        }

        if (char.IsLetter(Current))
        {
            int start = _position;

            while (char.IsLetter(Current))
            {
                Next();
            }

            int length = _position - start;
            string text = _text.Substring(start, length);
            SyntaxKind kind = SyntaxFacts.GetKeywordKind(text);

            return new SyntaxToken(kind, start, text, null);
        }

        switch (Current)
        {
            case '+': return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            case '-': return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
            case '*': return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
            case '/': return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
            case '(': return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
            case ')': return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
            default:
                {
                    _diagnostics.Add($"Error: Bad character input: {Current}");
                    return new SyntaxToken(SyntaxKind.InvalidToken, _position++, _text.Substring(_position - 1, 1), null);
                }
        }
    }

    private void Next()
    {
        _position++;
    }
}
