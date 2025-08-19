namespace Byl.Core.AST.Nodes;

// Параметр функции
public class ParameterNode : Node
{
    public string Name { get; }
    public TypeNode Type { get; }

    public ParameterNode(string name, TypeNode type, int line) : base(line)
    {
        Name = name;
        Type = type;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
