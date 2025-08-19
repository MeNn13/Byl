using Byl.Core.Lexer.TokenMatcher;

namespace Byl.Core.Lexer.Extensions;

[AttributeUsage(AttributeTargets.Field)]
internal class TokenMatcherAttribute : Attribute
{
    public Type MatcherType { get; }
    public int Priority { get; }

    public TokenMatcherAttribute(Type matcherType, int priority = 100)
    {
        if (!typeof(ITokenMatcher).IsAssignableFrom(matcherType))
            throw new ArgumentException("MatcherType must implement ITokenMatcher");

        MatcherType = matcherType;
        Priority = priority;
    }
}
