namespace Byl.Core.Lexer.Extensions;

[AttributeUsage(AttributeTargets.Field)]
public class KeywordAttribute(string russian) : Attribute
{
    public string RussianKeyword { get; } = russian;
}
