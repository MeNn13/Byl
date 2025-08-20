using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.AST.Nodes.Declaration;

public class FunctionDeclaration : DeclarationNode
{
    public bool IsStatic { get; }
    public TypeNode? ReturnType { get; }
    public string Name { get; }
    public List<ParameterNode> Parameters { get; }
    public BlockStatement Body { get; }

    public FunctionDeclaration(bool isStatic,
        TypeNode? returnType,
        string name,
        List<ParameterNode> parameters, 
        BlockStatement body,
        int line) : base(line)
    {
        IsStatic = isStatic;
        Name = name;
        Parameters = parameters;
        Body = body;
        ReturnType = returnType;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
