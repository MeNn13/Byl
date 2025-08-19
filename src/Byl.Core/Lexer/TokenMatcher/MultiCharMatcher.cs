namespace Byl.Core.Lexer.TokenMatcher;

public class MultiCharMatcher : ITokenMatcher
{
    private readonly string _pattern;
    private readonly int _patternLength;
    private readonly TokenType _type;

    public MultiCharMatcher(string pattern, TokenType type)
    {
        _pattern = pattern;
        _patternLength = _pattern.Length;
        _type = type;
    }

    public int Priority => 50;

    public Token? Match(LexerContext ctx)
    {
        if (ctx.Position + _patternLength > ctx.Code.Length)
            return null;

        int line = ctx.Line;

        for (int i = 0; i < _patternLength; i++)
        {
            if (ctx.Code[ctx.Position + i] == '\n')
                ctx.Line++;

            if (ctx.Code[ctx.Position + i] != _pattern[i])
                return null;
        }

        var token = new Token(_type, _pattern, line);
        ctx.Position += _patternLength;
        return token;
    }
}
