using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.Lexer;
using Byl.Core.Parser.Parsers;
using Byl.Core.Parser.Parsers.Declaration;
using Byl.Core.Parser.Utils;

namespace Byl.Core.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _position;
    internal Token Current => _tokens[_position];
    internal Token LastToken => _tokens[^1];
    internal bool IsAtEnd => _position >= _tokens.Count;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    public ProgramNode ParseProgram()
    {
        var program = new ProgramNode();

        while (Current.Type is not TokenType.EOF)
        {
            var declaration = ParseDeclaration();
            program.AddDeclaration(declaration);
        }

        return program;
    }

    internal bool Match(params TokenType[] types)
        => types.Any(t => Current.Type == t);
    internal Token Consume(TokenType type, string errorMessage)
    {
        if (Current.Type == type)
            return Advance();
        throw new ParserException(errorMessage, Current.Line);
    }
    internal Token? Peek(int count = 1) =>
        _position + count < _tokens.Count ? _tokens[_position + count] : null;
    internal Token Advance()
    {
        if (!IsAtEnd) _position++;
        return _tokens[_position - 1];
    }
    internal ParserException UnexpectedToken(Token token) =>
        new($"Неожиданный токен: {token.Type} '{token.Value}'", token.Line);
    internal bool IsTypeToken(TokenType type) => type switch
    {
        TokenType.VarType or TokenType.IntType or
        TokenType.StringType or TokenType.BoolType => true,
        _ => false
    };

    private DeclarationNode ParseDeclaration()
    {
        if (Match(TokenType.Namespace))
            return new NamespaceParser(this).Parse();

        if (Match(TokenType.Import))
            return new ImportParser(this).Parse();

        if (Match(TokenType.Class))
            return new ClassParser(this).Parse();

        throw UnexpectedToken(Current);
    }
}
