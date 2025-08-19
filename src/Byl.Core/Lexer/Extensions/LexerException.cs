namespace Byl.Core.Lexer.Extensions;

public class LexerException : Exception
{
    public int Position { get; }
    public int Line { get; }

    public LexerException(string message, int pos, int line)
        : base($"{message} (Позиция: {pos}, Строка: {line})")
    {
        Position = pos;
        Line = line;
    }
}
