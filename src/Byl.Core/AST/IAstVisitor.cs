using System.Linq.Expressions;
using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.AST;

public interface IAstVisitor<TResult>
{
    // Types
    TResult Visit(TypeNode node);

    // Declarations
    TResult Visit(ProgramNode node);
    TResult Visit(FunctionDeclaration node);
    TResult Visit(ParameterNode node);

    // Statements
    TResult Visit(BlockStatement node);
    TResult Visit(PrintStatement node);
    TResult Visit(AssignStatement node);
    TResult Visit(IfStatement node);
    TResult Visit(WhileStatement node);
    TResult Visit(ReturnStatement node);
    TResult Visit(VariableDeclarationStatement node);

    // Expressions
    TResult Visit(Nodes.Expression.BinaryExpression node);
    TResult Visit(LiteralExpression node);
    TResult Visit(VariableExpression node);
    TResult Visit(Nodes.Expression.UnaryExpression node);
}
