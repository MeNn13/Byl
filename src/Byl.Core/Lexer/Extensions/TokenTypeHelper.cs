using System.Reflection;

namespace Byl.Core.Lexer.Extensions;

public static class TokenTypeHelper
{
    public static string GetRussianKeyword(this TokenType type)
    {
        var field = type.GetType().GetField(type.ToString());
        var attr = field?.GetCustomAttribute<KeywordAttribute>();
        return attr?.RussianKeyword;
    }

    public static char GetSymbol(this TokenType type)
    {
        var field = type.GetType().GetField(type.ToString());
        var attr = field?.GetCustomAttribute<CharAttribute>();
        return attr.Symbol;
    }

    public static string GetString(this TokenType type)
    {
        var field = type.GetType().GetField(type.ToString());
        var attr = field?.GetCustomAttribute<StringAttribute>();
        return attr.String;
    }

    public static string GetMultiPattern(this TokenType type)
    {
        var field = type.GetType().GetField(type.ToString());
        var attr = field?.GetCustomAttribute<MultiCharAttribute>();
        return attr?.Pattern;
    }

    public static bool IsMulti(this TokenType type)
        => type.GetMultiPattern() is not null;

    public static bool IsKeyword(this TokenType type)
        => type.GetRussianKeyword() is not null;

    public static bool IsSymbol(this TokenType type)
        => type.GetSymbol() != null && !type.IsKeyword();

    public static bool IsString(this TokenType type)
        => type.GetString() is not null && !type.IsKeyword();
}
