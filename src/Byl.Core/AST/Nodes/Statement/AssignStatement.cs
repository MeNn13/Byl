using Byl.Core.AST.Nodes.Expression;

namespace Byl.Core.AST.Nodes.Statement;

public class AssignStatement : StatementNode
{
    public string VariableName { get; }
    public ExpressionNode Value { get; }

    public AssignStatement(string variableName, ExpressionNode value, int line) : base(line)
    {
        VariableName = variableName;
        Value = value;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
