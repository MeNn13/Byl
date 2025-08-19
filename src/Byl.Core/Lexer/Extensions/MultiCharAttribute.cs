namespace Byl.Core.Lexer.Extensions;

[AttributeUsage(AttributeTargets.Field)]
public class MultiCharAttribute(string pattern) : Attribute
{
    public string Pattern { get; } = pattern;
}
