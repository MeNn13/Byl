namespace Byl.Core.AST.Nodes.Declaration;

public class ClassDeclaration : DeclarationNode
{
    public string AccessModifier { get; }
    public string Name { get; }
    public List<FieldDeclaration> Fields { get; }
    public List<MethodDeclaration> Methods { get; }
    public ConstructorDeclaration? Constructor { get; }
    public string? BaseClass { get; }

    public ClassDeclaration(
        string accessModifier,
        string name,
        string? baseClass,
        List<FieldDeclaration>
        fields,
        ConstructorDeclaration? construct,
        List<MethodDeclaration> functions,
        int line) : base(line)
    {
        AccessModifier = accessModifier;
        Name = name;
        Fields = fields;
        Methods = functions;
        Constructor = construct;
        BaseClass = baseClass;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
