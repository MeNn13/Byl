using System.Text;
using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.AST.Visitors;

public class CodeGenerator : IAstVisitor<string>
{
    public string Visit(TypeNode node)
    {
        throw new NotImplementedException();
    }
    public string Visit(ProgramNode node)
    {
        var sb = new StringBuilder();
        sb.AppendLine("#include <stdio.h>");
        sb.AppendLine("int main() {");

        // Ищем главную функцию
        var mainFunc = node.Functions.FirstOrDefault(f => f.Name == "главный");
        if (mainFunc != null)
        {
            sb.Append(mainFunc.Body.Accept(this));
        }

        sb.AppendLine("    return 0;");
        sb.AppendLine("}");
        return sb.ToString();
    }
    public string Visit(FunctionDeclaration node)
    {
        var sb = new StringBuilder();

        // Генерируем сигнатуру функции на C
        sb.AppendLine($"void {node.Name}() {{");

        // Генерируем тело функции
        sb.Append(node.Body.Accept(this));

        sb.AppendLine("}");
        return sb.ToString();
    }
    public string Visit(ParameterNode node)
    {
        throw new NotImplementedException();
    }
    public string Visit(BlockStatement node)
    {
        var sb = new StringBuilder();
        foreach (var stmt in node.Statements)
        {
            sb.Append(stmt.Accept(this));
        }
        return sb.ToString(); ;
    }
    public string Visit(PrintStatement node)
    {
        return $"    printf(\"%d\\n\", {node.Expression.Accept(this)});\n";
    }
    public string Visit(AssignStatement node)
    {
        return $"    {node.VariableName} = {node.Value.Accept(this)};\n";
    }
    public string Visit(IfStatement node)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"    if ({node.Condition.Accept(this)}) {{");
        sb.Append(node.ThenBranch.Accept(this));
        sb.AppendLine("    }");

        if (node.ElseBranch != null)
        {
            sb.AppendLine("    else {");
            sb.Append(node.ElseBranch.Accept(this));
            sb.AppendLine("    }");
        }

        return sb.ToString();
    }
    public string Visit(WhileStatement node)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"    while ({node.Condition.Accept(this)}) {{");
        sb.Append(node.Body.Accept(this));
        sb.AppendLine("    }");
        return sb.ToString();
    }
    public string Visit(ReturnStatement node)
    {
        return node.Value != null
            ? $"    return {node.Value.Accept(this)};\n"
            : "    return;\n";
    }
    public string Visit(VariableDeclarationStatement node)
    {
        var init = node.Initializer != null ? $" = {node.Initializer.Accept(this)}" : "";
        return $"    int {node.VariableName}{init};\n";
    }
    public string Visit(BinaryExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        return node.Operator.Type switch
        {
            TokenType.Plus => $"({left} + {right})",
            TokenType.Minus => $"({left} - {right})",
            TokenType.Multiply => $"({left} * {right})",
            TokenType.Divide => $"({left} / {right})",
            TokenType.Equal => $"({left} == {right})",
            TokenType.NotEqual => $"({left} != {right})",
            TokenType.GreaterThan => $"({left} > {right})",
            _ => $"({left} {node.Operator.Value} {right})"
        };
    }
    public string Visit(LiteralExpression node)
    {
        return node.Value.ToString()?.ToLower() ?? "0";
    }
    public string Visit(VariableExpression node)
    {
        return node.Name;
    }
    public string Visit(UnaryExpression node)
    {
        var right = node.Right.Accept(this);
        return node.Operator.Type switch
        {
            TokenType.Minus => $"(-{right})",
            TokenType.Not => $"(!{right})",
            _ => $"({node.Operator.Value}{right})"
        };
    }
}
