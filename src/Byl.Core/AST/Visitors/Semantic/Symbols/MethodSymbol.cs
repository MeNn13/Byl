using Byl.Core.AST.Nodes.Declaration;

namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public class MethodSymbol : Symbol
{
    public MethodDeclaration Declaration { get; }

    public MethodSymbol(MethodDeclaration declaration)
        : base(declaration.Name, "method", declaration.Line) => Declaration = declaration;
}
