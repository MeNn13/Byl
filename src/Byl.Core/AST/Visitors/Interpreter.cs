using System.Text;
using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.AST.Visitors;

public class Interpreter : IAstVisitor<object>
{
    private readonly Dictionary<string, object> _variables = [];

    public object Visit(TypeNode node)
    {
        throw new NotImplementedException();
    }
    public object Visit(ProgramNode node)
    {
        foreach (var func in node.Functions)
        {
            if (func.Name == "главный")
                return func.Accept(this);
        }
        return null;
    }
    public object Visit(FunctionDeclaration node)
    {
        return node.Body.Accept(this);
    }
    public object Visit(ParameterNode node)
    {
        throw new NotImplementedException();
    }
    public object Visit(BlockStatement node)
    {
        object result = null;
        foreach (var stmt in node.Statements)
        {
            result = stmt.Accept(this);
        }
        return result;
    }
    public object Visit(PrintStatement node)
    {
        var value = node.Expression.Accept(this);
        Console.WriteLine(value);
        return value;
    }
    public object Visit(AssignStatement node)
    {
        var value = node.Value.Accept(this);
        _variables[node.VariableName] = value;
        return value;
    }
    public object Visit(IfStatement node)
    {
        var condition = (bool)node.Condition.Accept(this);
        return condition
            ? node.ThenBranch.Accept(this)
            : node.ElseBranch?.Accept(this);
    }
    public object Visit(WhileStatement node)
    {
        object result = null;
        while ((bool)node.Condition.Accept(this))
        {
            result = node.Body.Accept(this);
        }
        return result;
    }
    public object Visit(ReturnStatement node)
    {
        return node.Value?.Accept(this);
    }
    public object Visit(VariableDeclarationStatement node)
    {
        object value = node.Initializer?.Accept(this);
        _variables[node.VariableName] = value;
        return value;
    }
    public object Visit(BinaryExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        return node.Operator.Type switch
        {
            TokenType.Plus => (int)left + (int)right,
            TokenType.Minus => (int)left - (int)right,
            // ... другие операции
            _ => 0
        };
    }
    public object Visit(LiteralExpression node)
    {
        return node.Value;
    }
    public object Visit(VariableExpression node)
    {
        return _variables.TryGetValue(node.Name, out var value)
            ? value
            : throw new Exception($"Переменная '{node.Name}' не определена");
    }
    public object Visit(UnaryExpression node)
    {
        var right = node.Right.Accept(this);
        return node.Operator.Type switch
        {
            TokenType.Minus => -(int)right,
            TokenType.Not => !(bool)right,
            _ => right
        };
    }
}
