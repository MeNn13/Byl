using Byl.Core.Lexer;
using Byl.Core.Lexer.Extensions;
using Xunit;

namespace Byl.Tests.Lexer;

public class LexerTests
{
    private readonly Core.Lexer.Lexer _lexer = new();

    [Fact]
    public void Tokenizes_SimpleExpression()
    {
        var tokens = _lexer.Tokenize("главная() => { печать(\"test\"); }");

        Assert.Equal(11, tokens.Count);
        Assert.Equal(TokenType.Function, tokens[0].Type);
        Assert.Equal(TokenType.LParen, tokens[1].Type);
        Assert.Equal(TokenType.RParen, tokens[2].Type);
        Assert.Equal(TokenType.Arrow, tokens[3].Type);
        Assert.Equal(TokenType.LBrace, tokens[4].Type);
        Assert.Equal(TokenType.Print, tokens[5].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[7].Type);
        Assert.Equal(TokenType.Semicolon, tokens[9].Type);
        Assert.Equal(TokenType.RBrace, tokens[10].Type);
    }

    [Fact]
    public void Tokenizes_NumbersAndOperators()
    {
        var tokens = _lexer.Tokenize("42 + 3.14 * 2");

        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("42", tokens[0].Value);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal("3.14", tokens[2].Value);
        Assert.Equal(TokenType.Multiply, tokens[3].Type);
        Assert.Equal(TokenType.Number, tokens[4].Type);
    }

    [Fact]
    public void Skips_Comments()
    {
        var tokens = _lexer.Tokenize("печать(\"test\"); // комментарий");

        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.Print, tokens[0].Type);
        Assert.Equal(TokenType.LParen, tokens[1].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[2].Type);
        Assert.Equal("test", tokens[2].Value);
        Assert.Equal(TokenType.RParen, tokens[3].Type);
        Assert.Equal(TokenType.Semicolon, tokens[4].Type);
    }

    [Fact]
    public void Throws_OnUnknownCharacter()
    {
        var ex = Assert.Throws<LexerException>(() =>
            _lexer.Tokenize("печать(@)"));

        Assert.Contains("Неизвестный символ '@'", ex.Message);
        Assert.Equal(7, ex.Position);
    }
}
