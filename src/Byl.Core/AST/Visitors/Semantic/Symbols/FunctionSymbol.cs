using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;

namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public class FunctionSymbol : Symbol
{
    public List<ParameterNode> Parameters { get; }
    public TypeNode? ReturnType { get; }
    public FunctionDeclaration Declaration { get; }

    public FunctionSymbol(FunctionDeclaration declaration)
        : base(declaration.Name, "функция", declaration.Line)
    {
        Declaration = declaration;
        Parameters = declaration.Parameters;
        ReturnType = declaration.ReturnType;
    }
}
