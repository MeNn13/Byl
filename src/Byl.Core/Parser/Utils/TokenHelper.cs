using Byl.Core.Lexer;

namespace Byl.Core.Parser.Utils;

public static class TokenHelper
{
    public static bool Is(this Token token, TokenType type)
        => token.Type == type;

    public static void Ensure(this Token token, TokenType type, string error)
    {
        if (!token.Is(type))
            throw new ParserException(error, token.Line);
    }
}
