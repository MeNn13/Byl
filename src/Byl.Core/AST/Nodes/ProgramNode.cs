using Byl.Core.AST.Nodes.Declaration;
namespace Byl.Core.AST.Nodes;

// Программа - корневой узел
public class ProgramNode : Node
{
    private readonly List<FunctionDeclaration> _functions = [];
    public IReadOnlyCollection<FunctionDeclaration> Functions => _functions;

    public ProgramNode(List<FunctionDeclaration> functions, int line) : base(line)
    {
        _functions = functions;
    }

    public ProgramNode() : base(0) { }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);

    public void AddFunction(FunctionDeclaration function) => _functions.Add(function);
}
