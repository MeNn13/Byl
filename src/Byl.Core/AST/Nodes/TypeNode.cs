namespace Byl.Core.AST.Nodes;

public class TypeNode : Node
{
    public string TypeName { get; }

    public TypeNode(string typeName, int line) : base(line)
    {
        TypeName = typeName;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
