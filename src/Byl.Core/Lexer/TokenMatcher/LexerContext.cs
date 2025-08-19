namespace Byl.Core.Lexer.TokenMatcher;

public sealed class LexerContext
{
    public string Code { get; }
    public int Position { get; set; }
    public int Line { get; set; } = 1;

    public LexerContext(string code)
    {
        Code = code.Replace("\0", "");
    }

    public char Current => Position < Code.Length ? Code[Position] : '\0';

    public char Peek(int ahead = 1)
    {
        int pos = Position + ahead;
        return pos < Code.Length ? Code[pos] : '\0';
    }
}
