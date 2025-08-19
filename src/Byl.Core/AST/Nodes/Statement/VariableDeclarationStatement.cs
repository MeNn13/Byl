using Byl.Core.AST.Nodes.Expression;

namespace Byl.Core.AST.Nodes.Statement;

public class VariableDeclarationStatement : StatementNode
{
    public TypeNode Type { get; }
    public string VariableName { get; }
    public ExpressionNode? Initializer { get; }

    public VariableDeclarationStatement(TypeNode type, string variableName,
                                      ExpressionNode? initializer, int line)
        : base(line)
    {
        Type = type;
        VariableName = variableName;
        Initializer = initializer;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}