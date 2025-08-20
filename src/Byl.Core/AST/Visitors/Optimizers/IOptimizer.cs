using Byl.Core.AST.Nodes;

namespace Byl.Core.AST.Visitors.Optimizers;

public interface IOptimizer
{
    string Name { get; }
    Node Visit(Node node);
    bool CanOptimize(Node node);
}
