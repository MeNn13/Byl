using Byl.Core.Lexer;
using Byl.Core.Lexer.Extensions;
using Byl.Core.Lexer.TokenMatcher;
using Xunit;

namespace Byl.Tests.Lexer;

public class StringMatcherTests
{
    [Theory]
    [InlineData("\"текст\"", "текст")]
    [InlineData("\"с \\nпереводом\"", "с \nпереводом")]
    [InlineData("\"экранированная \\\"кавычка\\\"\"", "экранированная \"кавычка\"")]
    public void Matches_ValidStrings(string input, string expected)
    {
        var ctx = new LexerContext(input);
        var matcher = new StringMatcher();

        var token = matcher.Match(ctx);

        Assert.NotNull(token);
        Assert.Equal(TokenType.StringLiteral, token.Type);
        Assert.Equal(expected, token.Value);
    }

    [Fact]
    public void Throws_OnUnclosedString()
    {
        var ctx = new LexerContext("\"незакрытая");
        var matcher = new StringMatcher();

        var ex = Assert.Throws<LexerException>(() => matcher.Match(ctx));
        Assert.Contains("Незакрытая строка", ex.Message);
    }
}
