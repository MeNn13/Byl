using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.AST.Nodes.Declaration;

public class FunctionDeclaration : DeclarationNode
{
    public string Name { get; }
    public List<ParameterNode> Parameters { get; }
    public BlockStatement Body { get; }
    public TypeNode? ReturnType { get; }

    public FunctionDeclaration(string name,
        List<ParameterNode> parameters, 
        BlockStatement body,
        TypeNode? returnType,
        int line) : base(line)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
        ReturnType = returnType;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
