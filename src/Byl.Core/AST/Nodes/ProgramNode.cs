using Byl.Core.AST.Nodes.Declaration;
namespace Byl.Core.AST.Nodes;
public class ProgramNode : Node
{
    private readonly List<DeclarationNode> _declarations = [];
    public IReadOnlyCollection<DeclarationNode> Declarations => _declarations;

    public ProgramNode(List<DeclarationNode> declarations, int line) : base(line)
    {
        _declarations = declarations;
    }

    public ProgramNode() : base(0) { }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);

    public void AddDeclaration(DeclarationNode declaration) => _declarations.Add(declaration);
}