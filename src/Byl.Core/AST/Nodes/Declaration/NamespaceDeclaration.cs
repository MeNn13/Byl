namespace Byl.Core.AST.Nodes.Declaration;

public class NamespaceDeclaration : DeclarationNode
{
    public string Name { get; }
    public List<DeclarationNode> Members { get; }

    public NamespaceDeclaration(string name, List<DeclarationNode> members, int line) : base(line)
    {
        Name = name;
        Members = members;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
