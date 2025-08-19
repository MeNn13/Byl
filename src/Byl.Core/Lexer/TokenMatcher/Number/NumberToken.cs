namespace Byl.Core.Lexer.TokenMatcher.Number;

public class NumberToken : Token
{
    public NumberFormat Format { get; }

    public NumberToken(TokenType type, string value, NumberFormat format, int line)
        : base(type, value, line) => Format = format;
}
