using Byl.Core.Lexer.Extensions;

namespace Byl.Core.Lexer.TokenMatcher;

public sealed class SingleCharMatcher : ITokenMatcher
{
    private readonly char _char;
    private readonly TokenType _type;

    public SingleCharMatcher(TokenType type)
    {
        _char = type.GetSymbol();
        _type = type;
    }

    public int Priority => 100;

    public Token? Match(LexerContext ctx)
    {
        if (ctx.Current == _char)
        {
            var token = new Token(_type, _char.ToString(), ctx.Line);
            ctx.Position++;
            return token;
        }

        return null;
    }
}
