using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.AST.Visitors.Optimize;

public class UniversalOptimizer : IAstVisitor<Node>
{
    private readonly ConstantFolder _constantFolder = new();
    private readonly List<string> _optimizations = [];

    public IReadOnlyList<string> Optimizations => _optimizations;

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
        var optimizedExpression = node.Expression.Accept(_constantFolder);

        if (!ReferenceEquals(node.Expression, optimizedExpression))
        {
            _optimizations.Add($"Оптимизация печати {node.Line}");
        }

        return new PrintStatement(optimizedExpression, node.Line);
    }
    public Node Visit(AssignStatement node)
    {
        var optimizedValue = node.Value.Accept(_constantFolder);

        if (!ReferenceEquals(node.Value, optimizedValue))
        {
            _optimizations.Add($"Оптимизация названчения в строке {node.Line}");
        }

        return new AssignStatement(node.VariableName, optimizedValue, node.Line);
    }
    public Node Visit(IfStatement node)
    {
        var optimizedCondition = node.Condition.Accept(_constantFolder);
        var optimizedThen = (StatementNode)node.ThenBranch.Accept(this);
        var optimizedElse = node.ElseBranch != null ? (StatementNode)node.ElseBranch.Accept(this) : null;

        // Оптимизация: if (true) → thenBranch
        if (optimizedCondition is LiteralExpression { Value: true })
        {
            _optimizations.Add($"Убрано всегда истинное условие if в строке {node.Line}");
            return optimizedThen;
        }

        // Оптимизация: if (false) → elseBranch или пустой блок
        if (optimizedCondition is LiteralExpression { Value: false })
        {
            _optimizations.Add($"Убрано всегда ложное условие if в строке {node.Line}");
            return optimizedElse ?? new BlockStatement([], node.Line);
        }

        return new IfStatement(optimizedCondition, optimizedThen, optimizedElse, node.Line);
    }
    public Node Visit(WhileStatement node)
    {
        var optimizedCondition = node.Condition.Accept(_constantFolder);
        var optimizedBody = (StatementNode)node.Body.Accept(this);

        // Оптимизация: while (false) → пустой блок
        if (optimizedCondition is LiteralExpression { Value: false })
        {
            _optimizations.Add($"Удалено никогда не выполнявшееся условие while в строке {node.Line}");
            return new BlockStatement([], node.Line);
        }

        return new WhileStatement(optimizedCondition, optimizedBody, node.Line);
    }
    public Node Visit(ReturnStatement node)
    {
        ExpressionNode? optimizedValue = null;
        if (node.Value != null)
        {
            optimizedValue = node.Value.Accept(_constantFolder);

            if (!ReferenceEquals(node.Value, optimizedValue))
            {
                _optimizations.Add($"Возвращение в строке {node.Line}");
            }
        }

        return new ReturnStatement(optimizedValue, node.Line);
    }
    public Node Visit(VariableDeclarationStatement node)
    {
        ExpressionNode? optimizedInitializer = null;
        if (node.Initializer != null)
        {
            optimizedInitializer = node.Initializer.Accept(_constantFolder);

            if (!ReferenceEquals(node.Initializer, optimizedInitializer))
            {
                _optimizations.Add($"Инициализация переменной {node.Line}");
            }
        }

        return new VariableDeclarationStatement(node.Type, node.VariableName, optimizedInitializer, node.Line);
    }

    public Node Visit(BinaryExpression node) => _constantFolder.Visit(node);
    public Node Visit(LiteralExpression node) => _constantFolder.Visit(node);
    public Node Visit(VariableExpression node) => _constantFolder.Visit(node);
    public Node Visit(UnaryExpression node) => _constantFolder.Visit(node);

    public Node Visit(TypeNode node) => node;
    public Node Visit(ParameterNode node) => node;
}