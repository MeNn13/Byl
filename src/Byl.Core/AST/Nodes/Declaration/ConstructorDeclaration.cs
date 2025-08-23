using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.AST.Nodes.Declaration;

public class ConstructorDeclaration : DeclarationNode
{
    public string AccessModifier { get; }
    public List<ParameterNode> Parameters { get; }
    public BlockStatement Body { get; }

    public ConstructorDeclaration(string accessModifier, List<ParameterNode> parameters, BlockStatement body, int line) : base(line)
    {
        AccessModifier = accessModifier;
        Parameters = parameters;
        Body = body;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}