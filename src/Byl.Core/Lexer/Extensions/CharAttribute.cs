namespace Byl.Core.Lexer.Extensions;

[AttributeUsage(AttributeTargets.Field)]
public class CharAttribute(char symbol) : Attribute
{
    public char Symbol { get; } = symbol;
}
