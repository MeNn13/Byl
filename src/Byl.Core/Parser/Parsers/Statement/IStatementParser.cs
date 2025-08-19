using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.Parser.Parsers.Statement;

internal interface IStatementParser
{
    bool CanParse();
    StatementNode Parse();
}
