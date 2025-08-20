using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.AST.Visitors.Optimizers
{
    public class AstOptimizer : IAstVisitor<Node>
    {
        private readonly List<IOptimizer> _optimizer;
        private readonly List<string> _appliedOptimizations = [];

        public AstOptimizer()
        {
            _optimizer =
                [
                    new ConstantFoldingOptimizer(),
                    new DeadCodeOptimizer()
                ];
        }

        public IReadOnlyList<string> AppliedOptimizations => _appliedOptimizations;

        public Node Visit(TypeNode node) => node;
        public Node Visit(ProgramNode node)
        {
            var optimizedFunctions = new List<FunctionDeclaration>();
            foreach (var function in node.Functions)
            {
                var optimized = (FunctionDeclaration)function.Accept(this);
                optimizedFunctions.Add(optimized);
            }
            return new ProgramNode(optimizedFunctions, node.Line);
        }
        public Node Visit(FunctionDeclaration node)
        {
            var optimizedBody = (BlockStatement)node.Body.Accept(this);
            return new FunctionDeclaration(node.Name, node.Parameters, optimizedBody, node.ReturnType, node.Line);
        }
        public Node Visit(ParameterNode node) => node;
        public Node Visit(BlockStatement node)
        {
            var optimizedStatements = new List<StatementNode>();

            foreach (var statement in node.Statements)
            {
                var optimized = (StatementNode)statement.Accept(this);

                // Пропускаем пустые блоки
                if (optimized is BlockStatement block && block.Statements.Count == 0)
                    continue;

                optimizedStatements.Add(optimized);
            }

            return new BlockStatement(optimizedStatements, node.Line);
        }
        public Node Visit(PrintStatement node)
        {
            var optimizedExpression = (ExpressionNode)node.Expression.Accept(this);
            return new PrintStatement(optimizedExpression, node.Line);
        }
        public Node Visit(AssignStatement node)
        {
            var optimizedValue = (ExpressionNode)node.Value.Accept(this);
            return ApplyOptimizations(new AssignStatement(node.VariableName, optimizedValue, node.Line));
        }
        public Node Visit(IfStatement node)
        {
            var optimizedCondition = (ExpressionNode)node.Condition.Accept(this);
            var optimizedThen = (StatementNode)node.ThenBranch.Accept(this);
            var optimizedElse = node.ElseBranch is not null ? (StatementNode)node.ElseBranch.Accept(this) : null;

            return ApplyOptimizations(new IfStatement(optimizedCondition, optimizedThen, optimizedElse, node.Line));
        }
        public Node Visit(WhileStatement node)
        {
            var optimizedCondition = (ExpressionNode)node.Condition.Accept(this);
            var optimizedBody = (StatementNode)node.Body.Accept(this);
            return ApplyOptimizations(new WhileStatement(optimizedCondition, optimizedBody, node.Line));
        }
        public Node Visit(ReturnStatement node)
        {
            ExpressionNode? optimizedValue = null;
            if (node.Value != null)
            {
                optimizedValue = (ExpressionNode)node.Value.Accept(this);
            }
            return ApplyOptimizations(new ReturnStatement(optimizedValue, node.Line));
        }
        public Node Visit(VariableDeclarationStatement node)
        {
            ExpressionNode? optimizedInitializer = null;
            if (node.Initializer != null)
            {
                optimizedInitializer = (ExpressionNode)node.Initializer.Accept(this);
            }
            return ApplyOptimizations(new VariableDeclarationStatement(node.Type, node.VariableName, optimizedInitializer, node.Line));
        }
        public Node Visit(BinaryExpression node)
        {
            var left = (ExpressionNode)node.Left.Accept(this);
            var right = (ExpressionNode)node.Right.Accept(this);
            return ApplyOptimizations(new BinaryExpression(left, node.Operator, right, node.Line));
        }
        public Node Visit(LiteralExpression node) => node;
        public Node Visit(VariableExpression node) => node;
        public Node Visit(InterpolatedStringExpression node)
        {
            var optimizedParts = new List<ExpressionNode>();
            foreach (var part in node.Parts)
            {
                optimizedParts.Add((ExpressionNode)part.Accept(this));
            }
            return ApplyOptimizations(new InterpolatedStringExpression(optimizedParts, node.Line));
        }
        public Node Visit(UnaryExpression node)
        {
            var right = (ExpressionNode)node.Right.Accept(this);
            return ApplyOptimizations(new UnaryExpression(node.Operator, right, node.Line));
        }

        private T ApplyOptimizations<T>(T node) where T : Node
        {
            var current = node;
            foreach (var pass in _optimizer.Where(p => p.CanOptimize(current)))
            {
                var optimized = pass.Visit(current);
                if (!ReferenceEquals(optimized, current))
                {
                    _appliedOptimizations.Add($"{pass.Name} в строке {current.Line}");
                    current = (T)optimized;
                }
            }
            return current;
        }
    }
}
