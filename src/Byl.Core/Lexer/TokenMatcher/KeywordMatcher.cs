using Byl.Core.Lexer.Extensions;

namespace Byl.Core.Lexer.TokenMatcher;

public class KeywordMatcher : ITokenMatcher
{
    private static readonly Dictionary<string, TokenType> Keywords = [];

    static KeywordMatcher()
    {
        foreach (TokenType type in Enum.GetValues<TokenType>())
        {
            if (type.IsKeyword())
            {
                string keyword = type.GetRussianKeyword();
                if (!string.IsNullOrEmpty(keyword))
                {
                    Keywords[keyword] = type;
                }
            }
        }
    }

    public int Priority => 60;

    public Token? Match(LexerContext ctx)
    {
        if (!char.IsLetter(ctx.Current)) return null;

        int start = ctx.Position;
        while (ctx.Position < ctx.Code.Length &&
              (char.IsLetterOrDigit(ctx.Current) || ctx.Current == '_'))
        {
            ctx.Position++;
        }

        var text = ctx.Code[start..ctx.Position];

        return Keywords.TryGetValue(text, out var type)
            ? new Token(type, text, ctx.Line)
            : new Token(TokenType.Identifier, text, ctx.Line);

    }
}
