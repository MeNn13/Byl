using Byl.Core.AST.Nodes.Declaration;

namespace Byl.Core.AST.Visitors.Semantic.Symbols;

internal class ClassSymbol : Symbol
{
    public ClassDeclaration Declaration { get; }
    public string AccessModifier { get; }

    public ClassSymbol(string name, string accessModifier, int line)
        : base(name, "class", line) => AccessModifier = accessModifier;
}
