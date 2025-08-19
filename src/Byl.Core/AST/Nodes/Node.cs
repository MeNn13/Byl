namespace Byl.Core.AST.Nodes;

public abstract class Node(int line)
{
    public int Line { get; protected set; } = line;

    public abstract TResult Accept<TResult>(IAstVisitor<TResult> visitor);
}
