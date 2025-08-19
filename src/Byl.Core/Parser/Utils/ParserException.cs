namespace Byl.Core.Parser.Utils;

public class ParserException : Exception
{
    public int Line { get; }

    public ParserException(string message, int line)
        : base($"{message} строка {line}")
    {
        Line = line;
    }
}
