namespace Byl.Core.Lexer.Extensions;

[AttributeUsage(AttributeTargets.Field)]
public class StringAttribute(string str) : Attribute
{
    public string String { get; } = str;
}
