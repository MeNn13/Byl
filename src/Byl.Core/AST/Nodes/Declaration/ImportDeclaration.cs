namespace Byl.Core.AST.Nodes.Declaration;

public class ImportDeclaration : DeclarationNode
{
    public string Namespace { get; }

    public ImportDeclaration(string namespaceName, int line) : base(line)
    {
        Namespace = namespaceName;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}