namespace Sirius.CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.BangToken:
                return 6;

            default:
                return 0;
        }
    }

    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.StarToken:
            case SyntaxKind.SlashToken:
                return 5;

            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
                return 4;

            case SyntaxKind.EqualsEqualsToken:
            case SyntaxKind.BangEqualsToken:
            case SyntaxKind.LessToken:
            case SyntaxKind.LessOrEqualsToken:
            case SyntaxKind.GreaterToken:
            case SyntaxKind.GreaterOrEqualsToken:
                return 3;

            case SyntaxKind.AmpersandAmpersandToken:
                return 2;

            case SyntaxKind.PipePipeToken:
                return 1;

            default:
                return 0;
        }
    }

    public static SyntaxKind GetKeywordKind(string text)
    {
        return text switch
        {
            "else" => SyntaxKind.ElseKeyword,
            "false" => SyntaxKind.FalseKeyword,
            "for" => SyntaxKind.ForKeyword,
            "if" => SyntaxKind.IfKeyword,
            "let" => SyntaxKind.LetKeyword,
            "to" => SyntaxKind.ToKeyword,
            "true" => SyntaxKind.TrueKeyword,
            "var" => SyntaxKind.VarKeyword,
            "while" => SyntaxKind.WhileKeyword,
            _ => SyntaxKind.IdentifierToken,
        };
    }

    public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
    {
        var kinds = Enum.GetValues<SyntaxKind>();
        foreach (var kind in kinds)
        {
            if (GetUnaryOperatorPrecedence(kind) > 0)
                yield return kind;
        }
    }

    public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
    {
        var kinds = Enum.GetValues<SyntaxKind>();
        foreach (var kind in kinds)
        {
            if (GetBinaryOperatorPrecedence(kind) > 0)
                yield return kind;
        }
    }

    public static string GetText(SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.PlusToken => "+",
            SyntaxKind.MinusToken => "-",
            SyntaxKind.StarToken => "*",
            SyntaxKind.SlashToken => "/",
            SyntaxKind.BangToken => "!",
            SyntaxKind.EqualsToken => "=",
            SyntaxKind.LessToken => "<",
            SyntaxKind.LessOrEqualsToken => "<=",
            SyntaxKind.GreaterToken => ">",
            SyntaxKind.GreaterOrEqualsToken => ">=",
            SyntaxKind.AmpersandAmpersandToken => "&&",
            SyntaxKind.PipePipeToken => "||",
            SyntaxKind.EqualsEqualsToken => "==",
            SyntaxKind.BangEqualsToken => "!=",
            SyntaxKind.OpenParenthesisToken => "(",
            SyntaxKind.CloseParenthesisToken => ")",
            SyntaxKind.OpenBraceToken => "{",
            SyntaxKind.CloseBraceToken => "}",
            SyntaxKind.ElseKeyword => "else",
            SyntaxKind.FalseKeyword => "false",
            SyntaxKind.ForKeyword => "for",
            SyntaxKind.IfKeyword => "if",
            SyntaxKind.LetKeyword => "let",
            SyntaxKind.ToKeyword => "to",
            SyntaxKind.TrueKeyword => "true",
            SyntaxKind.VarKeyword => "var",
            SyntaxKind.WhileKeyword => "while",
            _ => null,
        };
    }
}
