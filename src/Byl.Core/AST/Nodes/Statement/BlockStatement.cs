namespace Byl.Core.AST.Nodes.Statement;

public class BlockStatement : StatementNode
{
    private readonly List<StatementNode> _statements = [];
    public IReadOnlyCollection<StatementNode> Statements => _statements;

    public BlockStatement(List<StatementNode> statements, int line) : base(line)
    {
        _statements = statements;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
