using Byl.Core.AST.Nodes.Expression;

namespace Byl.Core.AST.Nodes.Declaration;

public class FieldDeclaration : DeclarationNode
{
    public string AccessModifier { get; }
    public TypeNode Type { get; }
    public string Name { get; }
    public ExpressionNode? Initializer { get; }

    public FieldDeclaration(string accessModifier,
        TypeNode type,
        string name,
        ExpressionNode? initializer,
        int line) : base(line)
    {
        AccessModifier = accessModifier;
        Type = type;
        Name = name;
        Initializer = initializer;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
