using Byl.Core.AST.Nodes;

namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public class FunctionSymbol : Symbol
{
    public List<ParameterNode> Parameters { get; }
    public TypeNode? ReturnType { get; }

    public FunctionSymbol(string name, List<ParameterNode> parameters,
                        TypeNode? returnType, int line) : base(name, line)
    {
        Parameters = parameters;
        ReturnType = returnType;
    }
}
