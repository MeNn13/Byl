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
        // РАЗРЕШАЕМ КИРИЛЛИЧЕСКИЕ БУКВЫ!
        if (!char.IsLetter(ctx.Current) && !(ctx.Current > 127)) return null;

        int start = ctx.Position;

        // РАЗРЕШАЕМ КИРИЛЛИЧЕСКИЕ БУКВЫ В ИДЕНТИФИКАТОРАХ!
        while (ctx.Position < ctx.Code.Length &&
              (char.IsLetterOrDigit(ctx.Current) ||
               ctx.Current == '_' ||
               ctx.Current > 127)) // ← ВАЖНО: разрешаем Unicode символы
        {
            ctx.Position++;
        }

        var text = ctx.Code[start..ctx.Position];

        return Keywords.TryGetValue(text, out var type)
            ? new Token(type, text, ctx.Line)
            : new Token(TokenType.Identifier, text, ctx.Line);
    }
}
