using Byl.Core.AST.Nodes;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers;

public class TypeParser(Parser parser)
{
    private readonly Parser _parser = parser;

    public TypeNode Parse()
    {
        if (!_parser.IsTypeToken(_parser.Current.Type))
        {
            throw _parser.UnexpectedToken(_parser.Current);
        }

        var typeToken = _parser.Advance();
        return new TypeNode(typeToken.Value, typeToken.Line);
    }
}
